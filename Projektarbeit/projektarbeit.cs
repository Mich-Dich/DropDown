
namespace Projektarbeit {

    using Projektarbeit.player;
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Core;

    internal class projektarbeit : Game {

        public projektarbeit(String title, Int32 initalWindowWidth, Int32 initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        private CH_player CH_player;

        // ========================================================= functions =========================================================
        protected override void Init() {
            
            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            Set_Update_Frequency(144.0f);
            Show_DebugData(true);

            CH_player = new CH_player();
            this.player = CH_player;

            this.playerController = new PC_Default(CH_player);

            this.activeMap = new Base_Map();
            this.activeMap.Set_Background_Image("assets/textures/background/background.png");

            this.camera.Set_min_Max_Zoom(0.05f, 10.9f);
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

        protected override void Window_Resize() {

            this.camera.Set_Zoom(((float)this.window.Size.X / 3500.0f) + this.camera.zoom_offset);
        }

        protected override void Render(float deltaTime) { }
        
        protected override void Render_Imgui(float deltaTime) {
            // HUD
            ImGuiIOPtr io = ImGui.GetIO();
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;

            ImGui.SetNextWindowBgAlpha(1f);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(-2, this.window.Size.Y-38), ImGuiCond.Always, new System.Numerics.Vector2(0,1));
        }

    }
}
