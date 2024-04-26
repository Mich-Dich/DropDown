﻿using Core.controllers.player;
using Core.game_objects;
using Core.input;
using Core.physics;
using Core.renderer;
using Core.util;
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

        // ============================================================================== public ============================================================================== 
        public game(System.String title, Int32 inital_window_width, Int32 inital_window_height) {

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

            window.Unload += () => {
                
                // kill OpenGL
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.UseProgram(0);

                shutdown();
            };

            window.Load += () => {

                window.IsVisible = true;

                active_map = new map();

                init();
            };

            // internal game update
            window.UpdateFrame += (FrameEventArgs eventArgs) => {

                update_game_time(eventArgs.Time);
                this.player_controller.update_internal(game_time, _input_event);
                collision_engine.update(active_map.all_game_objects, game_time);       // call collision after update to force 

                update(game_time);                
                _input_event.Clear();
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {

                window.SwapBuffers();
                render(game_time);
            };

            window.Resize += (ResizeEventArgs eventArgs) => {

                update_game_time(window.TimeSinceLastUpdate());
                window.ResetTimeSinceLastUpdate();

                GL.Viewport(0, 0, window.Size.X, window.Size.Y);
                camera.set_view_size(window.Size);
                render(this.game_time);
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
                    _input_event.Add(new input_event(key_code.CursorPositionX, (KeyModifiers)0, (int)args.DeltaX, key_state.Repeat));

                if(args.DeltaY != 0)
                    _input_event.Add(new input_event(key_code.CursorPositionY, (KeyModifiers)0, (int)args.DeltaY, key_state.Repeat));
            };

            //collision_engine = new collision_engine();

            window.Run();
        }

        public static game instance {

            get {
                if(instance == null)
                    throw new Exception($"Instance not Initaliced");

                return instance;
            }
        }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int inital_window_width { get; set; }
        protected int inital_window_height { get; set; }

        protected GameWindow window { get; private set; }
        protected game_time game_time { get; } = new();
        protected collision_engine collision_engine { get; } = new();

        // game mode
        protected character plaer { get; set; }
        protected player_controller player_controller { get; set; }
        protected map active_map { get; set; }
        //protected List<map> active_maps { get; set; }     // TODO: add array of maps to enable DROP into new level

        // general
        protected camera camera { get; set; }

        protected abstract void init();
        protected abstract void shutdown();
        protected abstract void update(game_time delta_time);
        protected abstract void render(game_time delta_time);

        protected void set_update_frequency(double frequency) {

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = frequency;
        }

        // input system
        protected List<input_event> _input_event { get; }  = new List<input_event>();

        protected void reset_input_event_list() { _input_event.Clear(); }

        //  ============================================================================== private ============================================================================== 
        private game _instance;

        private void update_game_time(double delta_time) {

            this.game_time.elapsed = TimeSpan.FromMilliseconds(delta_time);
            this.game_time.total += TimeSpan.FromMilliseconds(delta_time);
        }
    }
}
