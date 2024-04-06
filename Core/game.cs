
using Core.controllers;
using Core.game_objects;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Core {
    public abstract class game {
        
        public GameWindowSettings _game_window_settings = GameWindowSettings.Default;
        public NativeWindowSettings _native_window_settings = NativeWindowSettings.Default;

        // ============================================================================== public ============================================================================== 
        public game(System.String title, Int32 inital_window_width, Int32 inital_window_height) {

            this.title = title;
            this.inital_window_width = inital_window_width;
            this.inital_window_height = inital_window_height;

            _native_window_settings.Size = new Vector2i(inital_window_width, inital_window_height);
            _native_window_settings.Title = title;
            _native_window_settings.StartVisible = false;
            _native_window_settings.StartFocused = true;
            //_native_window_settings.Vsync = VSyncMode.On;
            _native_window_settings.API = ContextAPI.OpenGL;
            _native_window_settings.Profile = ContextProfile.Core;
            _native_window_settings.Flags = ContextFlags.Default;

            _game_window_settings.UpdateFrequency = 60.0;
            
        }

        public void run() {
            
            init();
            window = new GameWindow(_game_window_settings, _native_window_settings);
            window.CenterWindow();


            window.Load += () => {

                window.IsVisible = true;
                load();
            };

            window.UpdateFrame += (FrameEventArgs eventArgs) => {

                update_game_time(eventArgs.Time);

                this.player_controller.update(game_time);
                update(game_time);
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {

                render(game_time);
                window.SwapBuffers();
            };

            window.Resize += (ResizeEventArgs eventArgs) => {

                update_game_time(window.TimeSinceLastUpdate());
                window.ResetTimeSinceLastUpdate();

                GL.Viewport(0, 0, window.Size.X, window.Size.Y);
                camera.set_view_size(window.Size);
                render(this.game_time);
                window.SwapBuffers();
            };

            window.KeyDown += key_down_test;
            window.MouseDown += mouse_bu_down_test;
            window.MouseMove += mouse_move;

            window.Unload += unload;
            window.Run();
        }

        private void update_game_time(double delta_time) {

            this.game_time.elapsed = TimeSpan.FromMilliseconds(delta_time);
            this.game_time.total += TimeSpan.FromMilliseconds(delta_time);
        }

        // ============================ input ============================ 
        public void key_down_test(KeyboardKeyEventArgs args) {

            // add new event to Queue
            Console.WriteLine("Key down: {0}", args.Key);
        }

        public void mouse_bu_down_test(MouseButtonEventArgs args) {

            // add new event to Queue
            Console.WriteLine("mouse_buttom down: {0}", args.Button);
        }

        public void mouse_move(MouseMoveEventArgs args) {

            // add new event to Queue
            Console.WriteLine("mouse_buttom down: {0}", args.Delta);
        }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int inital_window_width { get; set; }
        protected int inital_window_height { get; set; }

        protected player_controller player_controller { get; set; }
        protected GameWindow window { get; private set; }
        protected camera camera;
        protected game_time game_time = new();

        protected abstract void init();
        protected abstract void load();
        protected abstract void unload();
        protected abstract void update(game_time delta_time);
        protected abstract void render(game_time delta_time);

        //  ============================================================================== private ============================================================================== 


    }
}
