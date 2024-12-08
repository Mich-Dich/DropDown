
namespace Core {

    using Core.Controllers.player;
    using Core.defaults;
    using Core.Particles;
    using Core.render;
    using Core.render.shaders;
    using Core.util;
    using Core.world;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using System.Diagnostics;
    using System.Transactions;

    public enum Play_State {

        main_menu = 0,
        Playing = 1,
        dead = 2,
        skill_tree = 3,
        ability_skill_tree = 4,
        powerup_skill_tree = 5,
        InGameMenu = 6,
        LevelUp = 7,
        PauseMenuSkillTree = 8,
        PauseAbilitySkillTree = 9,
        PausePowerupSkillTree = 10
    }

    public class GameStateChangedEventArgs : EventArgs {

        public Play_State OldState { get; }
        public Play_State NewState { get; }

        public GameStateChangedEventArgs(Play_State oldState, Play_State newState) {
            OldState = oldState;
            NewState = newState;
        }
    }

    public static class DebugData {

        public static double workTimeUpdate = 0;
        public static double workTimeRender = 0;

        public static int spriteDrawCallsNum = 0;
        public static int numOfTiels = 0;
        public static int numOfTielsDisplayed = 0;
        public static int playingAnimationNum = 0;

        public static int colidableObjectsStatic = 0;
        public static int colidableObjectsDynamic = 0;

        public static int debug_lines = 0;
        public static int debug_circle = 0;
        public static int debug_rectangle = 0;

        public static void Reset() {

            workTimeUpdate = 0;
            workTimeRender = 0;
            spriteDrawCallsNum = 0;
            numOfTielsDisplayed = 0;
            playingAnimationNum = 0;
            debug_lines = 0;
            debug_circle = 0;
            debug_rectangle = 0;
        }
    }

    public abstract class Game {

        public Shader default_sprite_shader;
        public Shader particle_shader;
        public bool showDebug = false;
        public bool show_performance = false;
        public ImguI_Controller imguiController;
        public static Game Instance { get; private set; }
        public GameWindow window { get; private set; }
        public Camera camera { get; set; }
        public Character player { get; set; }
        public int Score { get; set; } = 0;
        public Play_State play_state = Play_State.main_menu;
        private global_debug_drawer global_Debug_Drawer { get; set; }

        protected string title { get; set; }
        protected int initalWindowWidth { get; set; }
        protected int initalWindowHeight { get; set; }
        public Player_Controller playerController { get; set; }
        protected Map activeMap { get; set; }

        public Map get_active_map() { return activeMap; }
        public void set_active_map(Map new_map) { activeMap = new_map; }

        protected List<InputEvent> inputEvent { get; } = new List<InputEvent>();

        private readonly Vector2 cursorPosOffset = new(0, 20);
        private readonly GameWindowSettings gameWindowSettings = GameWindowSettings.Default;
        private readonly NativeWindowSettings nativeWindowSettings = NativeWindowSettings.Default;
        private readonly DebugDataViualizer debugDataViualizer = new();
        private double updateFrequencyBuffer = 0;
        public Debug_Drawer Debug_Drawer;
        public GameState? GameState { get; set; }
        public event EventHandler<GameStateChangedEventArgs> GameStateChanged;


        public Game(string title, int initalWindowWidth, int initalWindowHeight) {

            if(Instance != null)
                throw new Exception("You can only create one instance of Game!");

            Instance = this;

            this.title = title;
            this.initalWindowWidth = initalWindowWidth;
            this.initalWindowHeight = initalWindowHeight;

            this.nativeWindowSettings.ClientSize = new Vector2i(initalWindowWidth, initalWindowHeight);
            this.nativeWindowSettings.Title = title;
            this.nativeWindowSettings.StartVisible = false;
            this.nativeWindowSettings.StartFocused = true;
            this.nativeWindowSettings.Vsync = VSyncMode.On;
            this.nativeWindowSettings.API = ContextAPI.OpenGL;
            this.nativeWindowSettings.Profile = ContextProfile.Core;
            this.nativeWindowSettings.Flags = ContextFlags.ForwardCompatible;
        }

        // ============================================================================== public ==============================================================================
        public abstract void StartGame();
        public virtual List<PowerUp> loadPowerups(List<PowerUpSaveData> PowerUpsSaveData) { return new List<PowerUp>(); }
        public virtual List<Ability> loadAbilities(List<AbilitySaveData> AbilitiesSaveData) { return new List<Ability>(); }

        public void Run() {

            this.window = new GameWindow(this.gameWindowSettings, this.nativeWindowSettings);
            this.window.CenterWindow();

            Stopwatch stopwatch = new();

            this.window.Load += () => {

                // ----------------------------------- defaults -----------------------------------
                GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                this.default_sprite_shader = new("Core.defaults.shaders.texture_vert.glsl", "Core.defaults.shaders.texture_frag.glsl", true);
                this.default_sprite_shader.Use();

                this.particle_shader = new("Core.defaults.shaders.particles.vert", "Core.defaults.shaders.particles.frag", true);
                this.particle_shader.Use();

                this.activeMap = new MAP_default();
                this.camera = new Camera(Vector2.Zero, this.window.Size, 0.5f);
                this.camera.Set_min_Max_Zoom(0.7f, 1.4f);
                this.camera.Set_Zoom(5.0f);
                this.InitImGuiController();

                this.Init();

                // ----------------------------------- check for null values -----------------------------------
                if(this.player == null)
                    this.player = new CH_default_player();

                if(this.playerController == null)
                    this.playerController = new PC_Default(player);


                // ----------------------------------- finish setup -----------------------------------
                this.playerController.character = this.player;
                if(!this.activeMap.player_is_spawned)
                    this.activeMap.Add_Character(this.player);
                this.window.IsVisible = true;
            };

            this.window.Unload += () => {

                // kill OpenGL
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.UseProgram(0);

                this.Shutdown();
            };

            // internal game update_internal
            this.window.UpdateFrame += (FrameEventArgs eventArgs) => {

                if(show_performance)
                    stopwatch.Restart();

                this.Update_Game_Time((float)eventArgs.Time);

                if(paused)
                    this.Update(Game_Time.delta);

                // proccess handeling
                this.playerController.Update_Internal(Game_Time.delta, this.inputEvent);
                this.inputEvent.Clear();

                this.activeMap.update_internal(Game_Time.delta);

                if(show_performance) {

                    stopwatch.Stop();
                    DebugData.workTimeUpdate = stopwatch.Elapsed.TotalMilliseconds;
                }
            };

            this.window.RenderFrame += (FrameEventArgs eventArgs) => {

                if(show_performance)
                    stopwatch.Restart();

                this.window.SwapBuffers();
                this.Internal_Render();
                this.Imgui_Render(Game_Time.delta);

                if(show_performance) {

                    stopwatch.Stop();
                    DebugData.workTimeRender = stopwatch.Elapsed.TotalMilliseconds;
                }

            };

            this.window.Resize += (ResizeEventArgs eventArgs) => {

                this.Update_Game_Time((float)this.window.TimeSinceLastUpdate());
                this.window.ResetTimeSinceLastUpdate();

                // update_internal the opengl viewport
                Vector2i fb = this.window.FramebufferSize;
                GL.Viewport(0, 0, fb.X, fb.Y);
                this.Window_Resize();

                this.imguiController.WindowResized(this.window.ClientSize.X, this.window.ClientSize.Y);

                this.camera.Set_View_Size(this.window.Size);
                this.Internal_Render();
                this.Imgui_Render(Game_Time.delta);
                this.window.SwapBuffers();
            };

            this.window.FocusedChanged += (e) => {

                if(!e.IsFocused) {

                    this.updateFrequencyBuffer = this.window.UpdateFrequency;
                    this.window.UpdateFrequency = 30.0f;
                }
                else
                    this.window.UpdateFrequency = this.updateFrequencyBuffer;
            };

            this.window.Move += (e) => {

                if(show_performance)
                    stopwatch.Restart();

                this.Update_Game_Time((float)this.window.TimeSinceLastUpdate());
                this.window.ResetTimeSinceLastUpdate();

                this.playerController.Update_Internal(Game_Time.delta, this.inputEvent);
                this.activeMap.update_internal(Game_Time.delta);
                this.Update(Game_Time.delta);
                this.inputEvent.Clear();

                if(show_performance) {

                    stopwatch.Stop();
                    DebugData.workTimeUpdate = stopwatch.Elapsed.TotalMilliseconds;
                    stopwatch.Restart();
                }

                this.window.SwapBuffers();
                this.Internal_Render();
                this.Imgui_Render(Game_Time.delta);

                if(show_performance) {

                    stopwatch.Stop();
                    DebugData.workTimeRender = stopwatch.Elapsed.TotalMilliseconds;
                }
            };

            // ============================ input ============================
            this.window.KeyDown += (KeyboardKeyEventArgs args) => { this.inputEvent.Add(new InputEvent((Key_Code)args.Key, args.Modifiers, args.IsRepeat ? 1 : 0, args.IsRepeat ? KeyState.Repeat : KeyState.Pressed)); };
            this.window.KeyUp += (KeyboardKeyEventArgs args) => { this.inputEvent.Add(new InputEvent((Key_Code)args.Key, args.Modifiers, args.IsRepeat ? 1 : 0, args.IsRepeat ? KeyState.Repeat : KeyState.Release)); };
            this.window.MouseDown += (MouseButtonEventArgs args) => { this.inputEvent.Add(new InputEvent((Key_Code)args.Button, args.Modifiers, args.Action == InputAction.Repeat ? 1 : 0, (KeyState)args.Action)); };
            this.window.MouseUp += (MouseButtonEventArgs args) => { this.inputEvent.Add(new InputEvent((Key_Code)args.Button, args.Modifiers, args.Action == InputAction.Repeat ? 1 : 0, (KeyState)args.Action)); };

            // make two events for X/Y of mouse wheel movement
            this.window.MouseWheel += (MouseWheelEventArgs args) => {

                if(args.OffsetX != 0)
                    this.inputEvent.Add(new InputEvent(Key_Code.MouseWheelX, (KeyModifiers)0, (int)args.Offset.X, KeyState.Repeat));

                if(args.OffsetY != 0)
                    this.inputEvent.Add(new InputEvent(Key_Code.MouseWheelY, (KeyModifiers)0, (int)args.Offset.Y, KeyState.Repeat));
            };

            // make two events for X/Y of mouse movement
            this.window.MouseMove += (MouseMoveEventArgs args) => {

                if(args.DeltaX != 0)
                    this.inputEvent.Add(new InputEvent(Key_Code.CursorPositionX, (KeyModifiers)0, (int)args.X, KeyState.Repeat));

                if(args.DeltaY != 0)
                    this.inputEvent.Add(new InputEvent(Key_Code.CursorPositionY, (KeyModifiers)0, (int)args.Y, KeyState.Repeat));
            };

            // collision_engine = new collision_engine();
            this.window.Run();
        }

        public void draw_debug_line(Vector2 start, Vector2 end, float duration_in_sec = 2.0f, DebugColor debugColor = DebugColor.Red) {

            if(showDebug && global_Debug_Drawer != null)
                global_Debug_Drawer.lines.Add(new debug_line(start, end, Game_Time.total, duration_in_sec, debugColor));
        }

        public void showDebugData(bool enable) {

            this.showDebug = enable;
            if(showDebug && global_Debug_Drawer == null)
                global_Debug_Drawer = new global_debug_drawer();

#if DEBUG
            Console.WriteLine($"Showing debug data");
#else
            if(enable)
                Console.WriteLine($"Showing debug data is enabled while building for release");
#endif
        }

        public void exit_game() { this.window.Close(); }

        public void pause(bool is_pause) { paused = is_pause; }

        public bool is_paused() { return paused; }

        public void Show_Performance(bool enable) { show_performance = enable; }

        public Vector2 Get_Mouse_Relative_Pos() { return this.window.MousePosition - (this.window.Size / 2) + this.cursorPosOffset; }

        // ============================================================================== protected ==============================================================================

        protected abstract void Init();
        protected abstract void Shutdown();
        protected abstract void Update(float deltaTime);
        protected abstract void Render(float deltaTime);
        protected abstract void Render_Imgui(float deltaTime);
        protected virtual void Window_Resize() { this.camera.Set_Zoom(((float)this.window.Size.X / 3500.0f) + this.camera.zoom_offset); }

        protected void Set_Update_Frequency(double frequency) {

            this.window.VSync = VSyncMode.Off;
            this.window.UpdateFrequency = frequency;
        }

        protected void ResetInputEvent_List() { this.inputEvent.Clear(); }

        protected virtual void OnGameStateChanged(Play_State oldState, Play_State newState) { GameStateChanged?.Invoke(this, new GameStateChangedEventArgs(oldState, newState)); }

        // ============================================================================== private ==============================================================================

        private bool paused = false;

        private void Internal_Render() {
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            this.default_sprite_shader.Use();
            this.default_sprite_shader.Set_Matrix_4x4("projection", this.camera.Get_Projection_Matrix());
            this.activeMap.Draw();

            if(this.showDebug) {

                this.activeMap.Draw_Debug();
                global_Debug_Drawer.draw();
            }

            this.particle_shader.Use();
            this.particle_shader.Set_Matrix_4x4("projection", this.camera.Get_Projection_Matrix());
            this.activeMap.draw_particles();

            this.Render(Game_Time.delta);
        }


        private void Update_Game_Time(float deltaTime) {

            Game_Time.delta = deltaTime;
            Game_Time.total += deltaTime;
        }

        // ============================================ IMGUI ============================================
        private void Imgui_Render(float deltaTime) {

            this.imguiController.Update(this.window, (float)Game_Time.delta);

            if(this.show_performance)
                this.debugDataViualizer.Draw();

            this.activeMap.Draw_Imgui();
            this.Render_Imgui(deltaTime);
            this.imguiController.Render();

            ImguI_Controller.CheckGLError("End of frame");
            DebugData.Reset();
        }

        private void InitImGuiController() {

            // Get the FrameBuffer size and compute the scale factor for ImGuiController
            Vector2i fb = this.window.FramebufferSize;
            int scaleFactorX = fb.X / this.window.ClientSize.X;
            int scaleFactorY = fb.Y / this.window.ClientSize.Y;

            // Instanciate the ImGuiController with the right Scale Factor
            this.imguiController = new ImguI_Controller(this.window.ClientSize.X, this.window.ClientSize.Y, scaleFactorX, scaleFactorY);
        }

    }
}
