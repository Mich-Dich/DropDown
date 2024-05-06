using Core.controllers.player;
using Core.game_objects;
using Core.input;
using Core.physics;
using Core.renderer;
using Core.util;
using ImGuiNET;
using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using Core.imgui;
using System.Diagnostics.Tracing;

namespace Core {

    public abstract class game {

        public GameWindowSettings   _game_window_settings = GameWindowSettings.Default;
        public NativeWindowSettings _native_window_settings = NativeWindowSettings.Default;

        public shader               default_sprite_shader;
        public static game          instance { get; private set; }
        public bool                 show_debug = true;
        public GameWindow           window { get; private set; }
        public camera               camera { get; set; }

        //public debug_data    debug_data { get; set; } = new debug_data();

        public ImGuiController      imguiController;

        // ============================================================================== public ============================================================================== 
        public game(System.String title, Int32 inital_window_width, Int32 inital_window_height) {

            if(instance != null)
                throw new Exception("You can only create one instance of Game!");

            instance = this;
            //this.ResourceManager = new ResourceManager();

            this.title = title;
            this.inital_window_width = inital_window_width;
            this.inital_window_height = inital_window_height;

            _native_window_settings.Size = new Vector2i(inital_window_width, inital_window_height);
            _native_window_settings.Title = title;
            _native_window_settings.StartVisible = false;
            _native_window_settings.StartFocused = true;
            _native_window_settings.Vsync = VSyncMode.On;
            _native_window_settings.API = ContextAPI.OpenGL;
            _native_window_settings.Profile = ContextProfile.Core;
            _native_window_settings.Flags = ContextFlags.ForwardCompatible;
        }

        public void run() {

            window = new GameWindow(_game_window_settings, _native_window_settings);
            window.CenterWindow();

            window.Load += () => {

                // ----------------------------------- defaults -----------------------------------
                GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));
                default_sprite_shader = new("shaders/texture_vert.glsl", "shaders/texture_frag.glsl", true);
                default_sprite_shader.use();
                camera = new(Vector2.Zero, this.window.Size, 0.5f);
                default_map = new map();

                init();

                // ----------------------------------- check for null values -----------------------------------
                if(active_map == null)
                    active_map = default_map;

                if(player == null)
                    player = new character();

                if(player_controller == null)
                    throw new ResourceNotAssignedException("player_controller musst be assigned in game class init() function");

                // ----------------------------------- finish setup -----------------------------------
                player_controller.player = player;
                this.active_map.add_game_object(player);
                initImGuiController();
                window.IsVisible = true;
            };

            window.Unload += () => {

                // kill OpenGL
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.UseProgram(0);

                shutdown();
            };

            // internal game update
            window.UpdateFrame += (FrameEventArgs eventArgs) => {

                update_game_time((float)eventArgs.Time);
                this.player_controller.update_internal(game_time.delta, _input_event);

                for(int x = 0; x < active_map.all_game_objects.Count; x++)
                    active_map.all_game_objects[x].update(game_time.delta);
                collision_engine.update(active_map.all_game_objects, game_time.delta);

                update(game_time.delta);
                _input_event.Clear();
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {

                window.SwapBuffers();
                internal_render();
                imgui_render(game_time.delta);
            };

            window.Resize += (ResizeEventArgs eventArgs) => {
                
                update_game_time((float)window.TimeSinceLastUpdate());
                window.ResetTimeSinceLastUpdate();

                // Update the opengl viewport
                Vector2i fb = this.window.FramebufferSize;
                GL.Viewport(0, 0, fb.X, fb.Y);
                window_resize();

                imguiController.WindowResized(this.window.ClientSize.X, this.window.ClientSize.Y);

                camera.set_view_size(window.Size);
                internal_render();
                imgui_render(game_time.delta);
                window.SwapBuffers();
            };



            // ============================ input ============================ 
            window.KeyDown += (KeyboardKeyEventArgs args) => { _input_event.Add(new input_event((key_code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? key_state.Repeat : key_state.Pressed)); };
            window.KeyUp += (KeyboardKeyEventArgs args) => { _input_event.Add(new input_event((key_code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? key_state.Repeat : key_state.Release)); };
            window.MouseDown += (MouseButtonEventArgs args) => { _input_event.Add(new input_event((key_code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (key_state)args.Action)); };
            window.MouseUp += (MouseButtonEventArgs args) => { _input_event.Add(new input_event((key_code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (key_state)args.Action)); };

            // make two events for X/Y of mouse wheel movement
            window.MouseWheel += (MouseWheelEventArgs args) => {

                if(args.OffsetX != 0)
                    _input_event.Add(new input_event(key_code.MouseWheelX, (KeyModifiers)0, (int)args.Offset.X, key_state.Repeat));

                if(args.OffsetY != 0)
                    _input_event.Add(new input_event(key_code.MouseWheelY, (KeyModifiers)0, (int)args.Offset.Y, key_state.Repeat));
            };

            // make two events for X/Y of mouse movement
            window.MouseMove += (MouseMoveEventArgs args) => {

                if(args.DeltaX != 0)
                    _input_event.Add(new input_event(key_code.CursorPositionX, (KeyModifiers)0, (int)args.X, key_state.Repeat));

                if(args.DeltaY != 0)
                    _input_event.Add(new input_event(key_code.CursorPositionY, (KeyModifiers)0, (int)args.Y, key_state.Repeat));
            };

            //collision_engine = new collision_engine();

            window.Run();
        }

        public void show_debug_data(bool enable) { this.show_debug = enable; }
        public Vector2 get_mouse_relative_pos() { return window.MousePosition - (window.Size / 2) + cursor_pos_offset; }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int inital_window_width { get; set; }
        protected int inital_window_height { get; set; }

        protected collision_engine collision_engine { get; } = new();
        protected map default_map { get; set; }
        protected character player { get; set; }
        protected player_controller player_controller { get; set; }
        protected map active_map { get; set; }
        //protected List<map> active_maps { get; set; }     // TODO: add array of maps to enable DROP into new level

        // general

        protected abstract void init();
        protected abstract void shutdown();
        protected abstract void update(float delta_time);
        protected abstract void render(float delta_time);
        protected abstract void render_imgui(float delta_time);
        protected virtual void window_resize() { }

        protected void set_update_frequency(double frequency) {

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = frequency;
        }

        // input system
        protected List<input_event> _input_event { get; } = new List<input_event>();

        protected void reset_input_event_list() { _input_event.Clear(); }

        //  ============================================================================== private ============================================================================== 
        private game _instance;
        private readonly Vector2 cursor_pos_offset = new Vector2(0,20);


        // ================ IMGUI TEST ================ 
        private float sliderFloatValue = 0.0f;
        private debug_data_viualizer debug_data_viualizer = new debug_data_viualizer();
        
        // ================ IMGUI TEST ================ 


        private void internal_render() {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            default_sprite_shader.use();
            default_sprite_shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            active_map.draw();
            //player.draw();

            if(show_debug) {

                //default_sprite_shader.set_matrix_4x4("projection", camera.get_projection_matrix());
                active_map.draw_denug();
            }

            // client side code
            render(game_time.delta);
        }

        private void update_game_time(float delta_time) {

            game_time.delta = delta_time;
            game_time.total += delta_time;

        }

        // ============================================ IMGUI ============================================

        private void imgui_render(float delta_time) {

            imguiController.Update(this.window, (float)game_time.delta);

            if(show_debug)
                debug_data_viualizer.draw();

            active_map.draw_imgui();
            render_imgui(delta_time);     // client side imgui code
            //ImGui.ShowDemoWindow();

            imguiController.Render();
            ImGuiController.CheckGLError("End of frame");

            debug_data.reset();
            //debug_data = new debug_data();  // reset debug infos
        }

        private void initImGuiController() {

            // Get the FrameBuffer size and compute the scale factor for ImGuiController
            Vector2i fb = this.window.FramebufferSize;
            int scaleFactorX = fb.X / this.window.ClientSize.X;
            int scaleFactorY = fb.Y / this.window.ClientSize.Y;

            // Instanciate the ImGuiController with the right Scale Factor
            imguiController = new ImGuiController(this.window.ClientSize.X, this.window.ClientSize.Y, scaleFactorX, scaleFactorY);
        }
    }

    public static class debug_data {

        public static int sprite_draw_calls_num = 0;
        public static int num_of_tiels = 0;
        public static int num_of_tiels_displayed = 0;
        public static int playing_animation_num = 0;
        
        public static int collision_checks_num = 0;
        public static int colidable_objects = 0;
        public static int colidable_objects_static = 0;
        public static int colidable_objects_dynamic = 0;

        //public debug_data() {}

        public static void reset() {
            
            sprite_draw_calls_num = 0;
            num_of_tiels_displayed = 0;
            playing_animation_num = 0;
            collision_checks_num = 0;
            colidable_objects = 0;
            colidable_objects_static = 0;
            colidable_objects_dynamic = 0;
    }

    }


}
