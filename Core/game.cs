using Core.controllers.player;
using Core.game_objects;
using Core.input;
using Core.physics;
using Core.renderer;
using Core.util;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Core
{

    public abstract class game {
        
        public GameWindowSettings _game_window_settings = GameWindowSettings.Default;
        public NativeWindowSettings _native_window_settings = NativeWindowSettings.Default;

        public shader           default_sprite_shader { get; set; }
        public static game      instance { get; private set; }
        //public ResourceManager  ResourceManager { get; private set; }
        public bool             draw_debug { get; set; } = false;

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
            _native_window_settings.Flags = ContextFlags.Default;

        }

        public void run() {
            
            window = new GameWindow(_game_window_settings, _native_window_settings);
            window.CenterWindow();

            window.Load += () => {

                GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));


                // ----------------------------------- shader -----------------------------------
                default_sprite_shader = new("shaders/texture_vert.glsl", "shaders/texture_frag.glsl", true);
                default_sprite_shader.use();

                // ----------------------------------- shader -----------------------------------
                camera = new(Vector2.Zero, this.window.Size, 0.5f);
                default_map = new map();

                init();

                if(active_map == null)
                    active_map = default_map;

                if (player == null)
                    player = new character();
                
                if (player_controller == null) 
                    throw new ResourceNotAssignedException("player_controller musst be assigned in game class init() function");    
                                
                player_controller.player = player;

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

                //Time.DeltaTime = e.Time;
                update_game_time((float)eventArgs.Time);
                this.player_controller.update_internal(_input_event);
                //collision_engine.update(active_map.all_game_objects);       // call collision after update to force 

                update();                
                _input_event.Clear();
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {

                window.SwapBuffers();
                internal_render();
            };

            window.Resize += (ResizeEventArgs eventArgs) => {

                update_game_time((float)window.TimeSinceLastUpdate());
                window.ResetTimeSinceLastUpdate();

                GL.Viewport(0, 0, window.Size.X, window.Size.Y);
                camera.set_view_size(window.Size);
                internal_render();
                window.SwapBuffers();
            };

            // ============================ input ============================ 
            window.KeyDown += (KeyboardKeyEventArgs args) => {      _input_event.Add(new input_event((key_code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? key_state.Repeat : key_state.Pressed)); };
            window.KeyUp += (KeyboardKeyEventArgs args) => {        _input_event.Add(new input_event((key_code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? key_state.Repeat : key_state.Release)); };
            window.MouseDown += (MouseButtonEventArgs args) => {    _input_event.Add(new input_event((key_code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (key_state)args.Action)); };
            window.MouseUp += (MouseButtonEventArgs args) => {      _input_event.Add(new input_event((key_code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (key_state)args.Action)); };

            // make two events for X/Y of mouse wheel movement
            window.MouseWheel += (MouseWheelEventArgs args) => {

                if(args.OffsetX != 0)
                    _input_event.Add(new input_event(key_code.MouseWheelX, (KeyModifiers)0, (int)args.OffsetX, key_state.Repeat));

                if(args.OffsetY != 0)
                    _input_event.Add(new input_event(key_code.MouseWheelY, (KeyModifiers)0, (int)args.OffsetY, key_state.Repeat));
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

        public void enable_debug_draw(bool enable) { this.draw_debug = enable; }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int inital_window_width { get; set; }
        protected int inital_window_height { get; set; }

        public GameWindow        window { get; private set; }
        //protected game_time         game_time { get; } = new();
        protected collision_engine  collision_engine { get; } = new();
        public camera            camera { get; set; }

        // default data
        protected map               default_map { get; set; }

        // game mode
        protected character         player { get; set; }
        protected player_controller player_controller { get; set; }
        protected map               active_map { get; set; }
        //protected List<map> active_maps { get; set; }     // TODO: add array of maps to enable DROP into new level

        // general

        protected abstract void init();
        protected abstract void shutdown();
        protected abstract void update();
        protected abstract void render();

        protected void set_update_frequency(double frequency) {

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = frequency;
        }

        // input system
        protected List<input_event> _input_event { get; }  = new List<input_event>();

        protected void reset_input_event_list() { _input_event.Clear(); }

        //  ============================================================================== private ============================================================================== 
        private game _instance;

        private void internal_render() {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            default_sprite_shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            active_map.draw();
            player.draw();

            // client side code
            render();
        }


        private void update_game_time(float delta_time) {

            game_time.elapsed = delta_time;
            game_time.total += delta_time;

        }
    }
}
