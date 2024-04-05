
using Core.game_objects;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core
{
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

            game_time game_time = new();

            window.Load += () => {

                window.IsVisible = true;
                load();
            };

            window.UpdateFrame += (FrameEventArgs eventArgs) => {

                double time = eventArgs.Time;
                game_time.elapsed = TimeSpan.FromMilliseconds(time);
                game_time.total += TimeSpan.FromMilliseconds(time);
                update(game_time);
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {

                render(game_time);
                window.SwapBuffers();
            };

            window.Resize += (ResizeEventArgs eventArgs) => {

                GL.Viewport(0, 0, window.Size.X, window.Size.Y);

                camera.set_view_size(window.Size);
                render(game_time);
                window.SwapBuffers();
            };

            //window.KeyDown += key_dow_test;
            window.Unload += unload;
            window.Run();
        }

        public void key_dow_test(KeyboardKeyEventArgs Args) {

            Console.WriteLine("Key down: {0}", Args.Key);
        }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int inital_window_width { get; set; }
        protected int inital_window_height { get; set; }

        protected GameWindow window { get; private set; }
        protected camera camera;

        protected abstract void init();
        protected abstract void load();
        protected abstract void unload();
        protected abstract void update(game_time delta_time);
        protected abstract void render(game_time delta_time);

        //  ============================================================================== private ============================================================================== 


    }
}
