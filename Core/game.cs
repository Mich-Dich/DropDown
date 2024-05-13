
namespace Core {

    using Core.controllers.player;
    using Core.input;
    using Core.render;
    using Core.render.shaders;
    using Core.util;
    using Core.world;
    using Core.world.map;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using System;
    using System.Diagnostics;

    public abstract class Game {

        public GameWindowSettings   _game_window_settings = GameWindowSettings.Default;
        public NativeWindowSettings _native_window_settings = NativeWindowSettings.Default;
        
        public Shader               default_sprite_shader;
        public static Game          instance { get; private set; }
        public bool                 show_debug = false;
        public GameWindow           window { get; private set; }
        public Camera               camera { get; set; }
        //public physics_engine       physics_engine { get; } = new();
        
        //public debug_data    debug_data { get; set; } = new debug_data();

        public Imgui_Controller      imguiController;

        // ============================================================================== public ============================================================================== 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.   => Value will be set in window.load function
        public Game(System.String title, Int32 inital_window_width, Int32 inital_window_height) {

            if(instance != null)
                throw new Exception("You can only create one instance of Game!");

            instance = this;
            //this.ResourceManager = new ResourceManager();

            this.title = title;
            this.inital_window_width = inital_window_width;
            this.inital_window_height = inital_window_height;

            _native_window_settings.ClientSize = new Vector2i(inital_window_width, inital_window_height);
            _native_window_settings.Title = title;
            _native_window_settings.StartVisible = false;
            _native_window_settings.StartFocused = true;
            _native_window_settings.Vsync = VSyncMode.On;
            _native_window_settings.API = ContextAPI.OpenGL;
            _native_window_settings.Profile = ContextProfile.Core;
            _native_window_settings.Flags = ContextFlags.ForwardCompatible;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void run() {

            window = new GameWindow(_game_window_settings, _native_window_settings);
            window.CenterWindow();

            Stopwatch stopwatch = new Stopwatch();

            window.Load += () => {

                // ----------------------------------- defaults -----------------------------------
                GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));
                default_sprite_shader = new("shaders/texture_vert.glsl", "shaders/texture_frag.glsl", true);
                default_sprite_shader.Use();
                camera = new(Vector2.Zero, this.window.Size, 0.5f);
                default_map = new Map();

                init();

                // ----------------------------------- check for null values -----------------------------------
                if (active_map == null)
                    active_map = default_map;

                if(player == null)
                    player = new Character();

                if(player_controller == null)
                    throw new ResourceNotAssignedException("player_controller musst be assigned in game class init() function");

                // ----------------------------------- finish setup -----------------------------------
                player_controller.character = player;
                this.active_map.Add_Character(player);

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

                stopwatch.Restart();

                update_game_time((float)eventArgs.Time);
                this.player_controller.update_internal(Game_Time.delta, _input_event);
                this.active_map.Update(Game_Time.delta);
                update(Game_Time.delta);
                _input_event.Clear();

                stopwatch.Stop();
                debug_data.work_time_update = stopwatch.Elapsed.TotalMilliseconds;
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {
                
                stopwatch.Restart();

                window.SwapBuffers();
                internal_render();
                imgui_render(Game_Time.delta);

                stopwatch.Stop();
                debug_data.work_time_render = stopwatch.Elapsed.TotalMilliseconds;
            };

            window.Resize += (ResizeEventArgs eventArgs) => {
                
                update_game_time((float)window.TimeSinceLastUpdate());
                window.ResetTimeSinceLastUpdate();

                // Update the opengl viewport
                Vector2i fb = this.window.FramebufferSize;
                GL.Viewport(0, 0, fb.X, fb.Y);
                window_resize();

                imguiController.WindowResized(this.window.ClientSize.X, this.window.ClientSize.Y);

                camera.Set_View_Size(window.Size);
                internal_render();
                imgui_render(Game_Time.delta);
                window.SwapBuffers();
            };

            window.FocusedChanged += (e) => {

                if(!e.IsFocused) {

                    update_frequency_buffer = window.UpdateFrequency;
                    window.UpdateFrequency = 30.0f;
                } else
                    window.UpdateFrequency = update_frequency_buffer;
            };

            window.Move += (e) => {

                stopwatch.Restart();
                
                {
                    update_game_time((float)window.TimeSinceLastUpdate());
                    window.ResetTimeSinceLastUpdate();

                    this.player_controller.update_internal(Game_Time.delta, _input_event);
                    this.active_map.Update(Game_Time.delta);
                    update(Game_Time.delta);
                    _input_event.Clear();
                }

                stopwatch.Stop();
                debug_data.work_time_update = stopwatch.Elapsed.TotalMilliseconds;
                stopwatch.Restart();
                
                {
                    window.SwapBuffers();
                    internal_render();
                    imgui_render(Game_Time.delta);
                }

                stopwatch.Stop();
                debug_data.work_time_render = stopwatch.Elapsed.TotalMilliseconds;
            };


            // ============================ input ============================ 
            window.KeyDown += (KeyboardKeyEventArgs args) => { _input_event.Add(new input_event((Key_Code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? key_state.Repeat : key_state.Pressed)); };
            window.KeyUp += (KeyboardKeyEventArgs args) => { _input_event.Add(new input_event((Key_Code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? key_state.Repeat : key_state.Release)); };
            window.MouseDown += (MouseButtonEventArgs args) => { _input_event.Add(new input_event((Key_Code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (key_state)args.Action)); };
            window.MouseUp += (MouseButtonEventArgs args) => { _input_event.Add(new input_event((Key_Code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (key_state)args.Action)); };

            // make two events for X/Y of mouse wheel movement
            window.MouseWheel += (MouseWheelEventArgs args) => {

                if(args.OffsetX != 0)
                    _input_event.Add(new input_event(Key_Code.MouseWheelX, (KeyModifiers)0, (int)args.Offset.X, key_state.Repeat));

                if(args.OffsetY != 0)
                    _input_event.Add(new input_event(Key_Code.MouseWheelY, (KeyModifiers)0, (int)args.Offset.Y, key_state.Repeat));
            };

            // make two events for X/Y of mouse movement
            window.MouseMove += (MouseMoveEventArgs args) => {

                if(args.DeltaX != 0)
                    _input_event.Add(new input_event(Key_Code.CursorPositionX, (KeyModifiers)0, (int)args.X, key_state.Repeat));

                if(args.DeltaY != 0)
                    _input_event.Add(new input_event(Key_Code.CursorPositionY, (KeyModifiers)0, (int)args.Y, key_state.Repeat));
            };

            //collision_engine = new collision_engine();

            window.Run();

        }

        public void show_debug_data(bool enable) {

            this.show_debug = enable;
#if DEBUG
            Console.WriteLine($"Showing debug data");
#else
            if(enable)
                Console.WriteLine($"Showing debug data is enabled while building for release");
#endif
        }
        public Vector2 get_mouse_relative_pos() { return window.MousePosition - (window.Size / 2) + cursor_pos_offset; }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int inital_window_width { get; set; }
        protected int inital_window_height { get; set; }

        protected Map default_map { get; set; }
        protected Character player { get; set; }
        protected player_controller player_controller { get; set; }
        protected Map active_map { get; set; }
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
        private readonly Vector2 cursor_pos_offset = new Vector2(0,20);
        private Debug_Data_Viualizer debug_data_viualizer = new Debug_Data_Viualizer();
        private double update_frequency_buffer = 0;

        private void internal_render() {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            default_sprite_shader.Use();
            default_sprite_shader.Set_Matrix_4x4("projection", camera.Get_Projection_Matrix());

            active_map.Draw();
            //player.Draw();

            if(show_debug) {

                //default_sprite_shader.Set_Matrix_4x4("projection", camera.get_projection_matrix());
                active_map.Draw_Debug();
            }

            // client side code
            render(Game_Time.delta);
        }

        private void update_game_time(float delta_time) {

            Game_Time.delta = delta_time;
            Game_Time.total += delta_time;

        }

        // ============================================ IMGUI ============================================

        private void imgui_render(float delta_time) {

            imguiController.Update(this.window, (float)Game_Time.delta);

            if(show_debug)
                debug_data_viualizer.Draw();

            active_map.Draw_Imgui();
            render_imgui(delta_time);     // client side imgui code
            //ImGui.ShowDemoWindow();

            imguiController.Render();
            Imgui_Controller.CheckGLError("End of frame");

            debug_data.reset();
            //debug_data = new debug_data();  // reset debug infos
        }

        private void initImGuiController() {

            // Get the FrameBuffer size and compute the scale factor for ImGuiController
            Vector2i fb = this.window.FramebufferSize;
            int scaleFactorX = fb.X / this.window.ClientSize.X;
            int scaleFactorY = fb.Y / this.window.ClientSize.Y;

            // Instanciate the ImGuiController with the right Scale Factor
            imguiController = new Imgui_Controller(this.window.ClientSize.X, this.window.ClientSize.Y, scaleFactorX, scaleFactorY);
        }
    }

    public static class debug_data {

        public static double work_time_update = 0;
        public static double work_time_render = 0;

        public static int sprite_draw_calls_num = 0;
        public static int num_of_tiels = 0;
        public static int num_of_tiels_displayed = 0;
        public static int playing_animation_num = 0;
        
        public static int collision_checks_num = 0;
        public static int colidable_objects = 0;
        public static int colidable_objects_static = 0;
        public static int colidable_objects_dynamic = 0;
        public static int collision_num = 0;

        //public debug_data() {}

        public static void reset() {

            work_time_update = 0;
            work_time_render = 0;
            sprite_draw_calls_num = 0;
            num_of_tiels_displayed = 0;
            playing_animation_num = 0;
            collision_checks_num = 0;
            colidable_objects = 0;
            colidable_objects_static = 0;
            colidable_objects_dynamic = 0;
            collision_num = 0;
        }

    }


}
