
namespace Hell {

    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using ImGuiNET;
    using Core.util;
    using Hell.player;
    using Hell.Levels;

    internal class Game : Core.Game {

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }


        // ========================================================= functions =========================================================
        protected override void Init() {

            Set_Update_Frequency(144.0f);
            this.activeMap = new MAP_base();
            this.player = new CH_player();
            this.playerController = new PC_main(player);
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

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
                ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, io.DisplaySize.Y - 10), ImGuiCond.Always, new System.Numerics.Vector2(0, 1));
                ImGui.Begin("HUD_TopLeft", window_flags);

                uint col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
                uint transparentColor = ImGui.GetColorU32(new System.Numerics.Vector4(0, 0, 0, 0));

                Imgui_Util.Progress_Bar_Stylised(this.player.health / this.player.health_max, new System.Numerics.Vector2(250, 15), col_red, transparentColor, 0.32f, 0.28f, 0.6f);

                ImGui.Spacing();
                Imgui_Util.Title($"Score: {Game.Instance.Score}");
                ImGui.End();
            }
    }
}
