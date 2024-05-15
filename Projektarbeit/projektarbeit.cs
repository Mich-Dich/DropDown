
namespace Hell {

    using Core.render;
    using Core.util;
    using Hell.player;
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    internal class Projektarbeit : Core.Game {

        public Projektarbeit(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) {
        
        }

        private CH_player CH_player;

        // ========================================================= functions =========================================================
        protected override void Init() {
            
            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            Set_Update_Frequency(144.0f);
            CH_player = new CH_player();
            this.playerController = new PC_Default(CH_player);
            this.player = CH_player;
            
            this.camera.Set_min_Max_Zoom(0.7f, 1.4f);
            this.camera.Set_Zoom(5.0f);
            this.activeMap = new Base_Map(this.camera);
            this.activeMap.Set_Background_Image("assets/textures/background/Background.png");
#if DEBUG
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.01f, 1.4f);
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

        protected override void Window_Resize() { this.camera.Set_Zoom(((float)this.window.Size.X / 2200.0f) + this.camera.zoom_offset); }

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
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoBackground;

            ImGui.SetNextWindowBgAlpha(0f);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, 10), ImGuiCond.Always, new System.Numerics.Vector2(0,0));
            ImGui.Begin("HUD_TopLeft", window_flags);

            uint col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
            uint transparentColor = ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0));

            Imgui_Util.Progress_Bar_Stylised(CH_player.health / CH_player.health_max, new System.Numerics.Vector2(250, 15), col_red, transparentColor, 0.32f, 0.28f, 0.6f);

            ImGui.Spacing();
            Imgui_Util.Title("Score");
            ImGui.SameLine();
            Imgui_Util.Title("0");

            ImGui.End();
        }
    }
}
