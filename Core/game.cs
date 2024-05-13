
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
    using System.Diagnostics;

    public abstract class Game {

        public GameWindowSettings   _game_window_settings = GameWindowSettings.Default;
        public NativeWindowSettings _native_window_settings = NativeWindowSettings.Default;
        
        public Shader               default_sprite_shader;
        public static Game          instance { get; private set; }
        public bool                 show_debug = true;
        public GameWindow           window { get; private set; }
        public Camera               camera { get; set; }
        //public Physics_Engine       Physics_Engine { get; } = new();
        
        //public Debug_Data    Debug_Data { get; set; } = new Debug_Data();

        public ImguI_Controller      imguiController;

        // ============================================================================== public ============================================================================== 

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.   => Value will be set in window.load function
        public Game(System.String title, Int32 initalWindowWidth, Int32 initalWindowHeight) {

            if(instance != null)
                throw new Exception("You can only create one instance of Game!");

            instance = this;
            //this.ResourceManager = new ResourceManager();

            this.title = title;
            this.initalWindowWidth = initalWindowWidth;
            this.initalWindowHeight = initalWindowHeight;

            _native_window_settings.ClientSize = new Vector2i(initalWindowWidth, initalWindowHeight);
            _native_window_settings.Title = title;
            _native_window_settings.StartVisible = false;
            _native_window_settings.StartFocused = true;
            _native_window_settings.Vsync = VSyncMode.On;
            _native_window_settings.API = ContextAPI.OpenGL;
            _native_window_settings.Profile = ContextProfile.Core;
            _native_window_settings.Flags = ContextFlags.ForwardCompatible;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Run() {

            window = new GameWindow(_game_window_settings, _native_window_settings);
            window.CenterWindow();

            Stopwatch stopwatch = new Stopwatch();

            window.Load += () => {

                // ----------------------------------- defaults -----------------------------------
                GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));
                default_sprite_shader = new("shaders/texture_vert.glsl", "shaders/texture_frag.glsl", true);
                default_sprite_shader.Use();
                camera = new(Vector2.Zero, this.window.Size, 0.5f);
                defaultMap = new Map();

                Init();

                // ----------------------------------- check for null values -----------------------------------
                if(activeMap == null)
                    activeMap = defaultMap;

                if(player == null)
                    player = new Character();

                if(playerController == null)
                    throw new ResourceNotAssignedException("playerController musst be assigned in game class Init() function");

                // ----------------------------------- finish setup -----------------------------------
                playerController.character = player;
                this.activeMap.Add_Character(player);

                InitImGuiController();
                window.IsVisible = true;
            };

            window.Unload += () => {

                // kill OpenGL
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.UseProgram(0);

                Shutdown();
            };

            // internal game Update
            window.UpdateFrame += (FrameEventArgs eventArgs) => {

                stopwatch.Restart();

                Update_Game_Time((float)eventArgs.Time);
                this.playerController.Update_Internal(Game_Time.delta, inputEvent);

                for(int x = 0; x < activeMap.allCollidableGameObjects.Count; x++)
                    activeMap.allCollidableGameObjects[x].Update(Game_Time.delta);

                //collision_engine.Update(activeMap.allCollidableGameObjects, game_time.delta);
                this.activeMap.Update(Game_Time.delta);

                Update(Game_Time.delta);
                inputEvent.Clear();

                stopwatch.Stop();
                Debug_Data.workTimeUpdate = stopwatch.Elapsed.TotalMilliseconds;
            };

            window.RenderFrame += (FrameEventArgs eventArgs) => {
                
                stopwatch.Restart();

                window.SwapBuffers();
                Internal_Render();
                Imgui_Render(Game_Time.delta);

                stopwatch.Stop();
                Debug_Data.workTimeRender = stopwatch.Elapsed.TotalMilliseconds;
            };

            window.Resize += (ResizeEventArgs eventArgs) => {
                
                Update_Game_Time((float)window.TimeSinceLastUpdate());
                window.ResetTimeSinceLastUpdate();

                // Update the opengl viewport
                Vector2i fb = this.window.FramebufferSize;
                GL.Viewport(0, 0, fb.X, fb.Y);
                Window_Resize();

                imguiController.WindowResized(this.window.ClientSize.X, this.window.ClientSize.Y);

                camera.Set_View_Size(window.Size);
                Internal_Render();
                Imgui_Render(Game_Time.delta);
                window.SwapBuffers();
            };



            // ============================ input ============================ 
            window.KeyDown += (KeyboardKeyEventArgs args) => { inputEvent.Add(new InputEvent((Key_Code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? KeyState.Repeat : KeyState.Pressed)); };
            window.KeyUp += (KeyboardKeyEventArgs args) => { inputEvent.Add(new InputEvent((Key_Code)args.Key, args.Modifiers, (args.IsRepeat ? 1 : 0), args.IsRepeat ? KeyState.Repeat : KeyState.Release)); };
            window.MouseDown += (MouseButtonEventArgs args) => { inputEvent.Add(new InputEvent((Key_Code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (KeyState)args.Action)); };
            window.MouseUp += (MouseButtonEventArgs args) => { inputEvent.Add(new InputEvent((Key_Code)args.Button, args.Modifiers, (args.Action == InputAction.Repeat ? 1 : 0), (KeyState)args.Action)); };

            // make two events for X/Y of mouse wheel movement
            window.MouseWheel += (MouseWheelEventArgs args) => {

                if(args.OffsetX != 0)
                    inputEvent.Add(new InputEvent(Key_Code.MouseWheelX, (KeyModifiers)0, (int)args.Offset.X, KeyState.Repeat));

                if(args.OffsetY != 0)
                    inputEvent.Add(new InputEvent(Key_Code.MouseWheelY, (KeyModifiers)0, (int)args.Offset.Y, KeyState.Repeat));
            };

            // make two events for X/Y of mouse movement
            window.MouseMove += (MouseMoveEventArgs args) => {

                if(args.DeltaX != 0)
                    inputEvent.Add(new InputEvent(Key_Code.CursorPositionX, (KeyModifiers)0, (int)args.X, KeyState.Repeat));

                if(args.DeltaY != 0)
                    inputEvent.Add(new InputEvent(Key_Code.CursorPositionY, (KeyModifiers)0, (int)args.Y, KeyState.Repeat));
            };

            //collision_engine = new collision_engine();

            window.Run();
        }

        public void Show_Debug_Data(bool enable) { this.show_debug = enable; }
        public Vector2 Get_Mouse_Relative_Pos() { return window.MousePosition - (window.Size / 2) + cursorPosOffset; }

        //  ============================================================================== protected ============================================================================== 
        protected string title { get; set; }
        protected int initalWindowWidth { get; set; }
        protected int initalWindowHeight { get; set; }

        protected Map defaultMap { get; set; }
        protected Character player { get; set; }
        protected Player_Controller playerController { get; set; }
        protected Map activeMap { get; set; }
        //protected List<map> activeMaps { get; set; }     // TODO: add array of maps to enable DROP into new level

        // general
        protected abstract void Init();
        protected abstract void Shutdown();
        protected abstract void Update(float deltaTime);
        protected abstract void Render(float deltaTime);
        protected abstract void Render_Imgui(float deltaTime);
        protected virtual void Window_Resize() { }

        protected void Set_Update_Frequency(double frequency) {

            window.VSync = VSyncMode.Off;
            window.UpdateFrequency = frequency;
        }

        // input system
        protected List<InputEvent> inputEvent { get; } = new List<InputEvent>();
        protected void ResetInputEvent_List() { inputEvent.Clear(); }

        //  ============================================================================== private ============================================================================== 
        private readonly Vector2 cursorPosOffset = new Vector2(0,20);
        private DebugDataViualizer debugDataViualizer = new DebugDataViualizer();


        private void Internal_Render() {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            default_sprite_shader.Use();
            default_sprite_shader.Set_Matrix_4x4("projection", camera.Get_Projection_Matrix());

            activeMap.Draw();
            //player.Draw();

            if(show_debug) {

                //default_sprite_shader.Set_Matrix_4x4("projection", camera.get_projection_matrix());
                activeMap.Draw_Debug();
            }

            // client side code
            Render(Game_Time.delta);
        }

        private void Update_Game_Time(float deltaTime) {

            Game_Time.delta = deltaTime;
            Game_Time.total += deltaTime;

        }

        // ============================================ IMGUI ============================================

        private void Imgui_Render(float deltaTime) {

            imguiController.Update(this.window, (float)Game_Time.delta);

            if(show_debug)
                debugDataViualizer.Draw();

            activeMap.Draw_Imgui();
            Render_Imgui(deltaTime);     // client side imgui code
            //ImGui.ShowDemoWindow();

            imguiController.Render();
            ImguI_Controller.CheckGLError("End of frame");

            Debug_Data.Reset();
            //Debug_Data = new Debug_Data();  // reset debug infos
        }

        private void InitImGuiController() {

            // Get the FrameBuffer size and compute the scale factor for ImGuiController
            Vector2i fb = this.window.FramebufferSize;
            int scaleFactorX = fb.X / this.window.ClientSize.X;
            int scaleFactorY = fb.Y / this.window.ClientSize.Y;

            // Instanciate the ImGuiController with the right Scale Factor
            imguiController = new ImguI_Controller(this.window.ClientSize.X, this.window.ClientSize.Y, scaleFactorX, scaleFactorY);
        }
    }

    public static class Debug_Data {

        public static double workTimeUpdate = 0;
        public static double workTimeRender = 0;

        public static int spriteDrawCallsNum = 0;
        public static int numOfTiels = 0;
        public static int numOfTiels_Displayed = 0;
        public static int playingAnimationNum = 0;
        
        public static int collisionChecksNum = 0;
        public static int colidableObjects = 0;
        public static int colidableObjects_Static = 0;
        public static int colidableObjects_Dynamic = 0;
        public static int collisionNum = 0;

        //public Debug_Data() {}

        public static void Reset() {

            workTimeUpdate = 0;
            workTimeRender = 0;
            spriteDrawCallsNum = 0;
            numOfTiels_Displayed = 0;
            playingAnimationNum = 0;
            collisionChecksNum = 0;
            colidableObjects = 0;
            colidableObjects_Static = 0;
            colidableObjects_Dynamic = 0;
            collisionNum = 0;
        }

    }


}
