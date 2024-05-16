
namespace DropDown {

    using Core;
    using Core.render;
    using Core.util;
    using DropDown.player;
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    internal class Drop_Down : Core.Game {

        public Drop_Down(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        private CH_player CH_player;
        private Texture image;

        private System.Numerics.Vector2 screen_size_buffer;

        // ========================================================= functions =========================================================
        protected override void Init() {

            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            Set_Update_Frequency(144.0f);
            CH_player = new CH_player();
            this.playerController = new PC_Default(CH_player);
            this.player = CH_player;
            this.activeMap = new MAP_base();
            this.camera.Set_min_Max_Zoom(0.7f, 1.4f);
            this.camera.Set_Zoom(5.0f);
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
            image = Resource_Manager.Get_Texture("assets/textures/BloodOverlay.png");
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

        protected override void Window_Resize() {

            base.Window_Resize();
            screen_size_buffer = util.convert_Vector(Game.Instance.window.ClientSize + new Vector2(3));
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {

            display_blood_overlay(deltaTime);

            ImGui.ShowStyleEditor();

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
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(-2, this.window.Size.Y - 38), ImGuiCond.Always, new System.Numerics.Vector2(0, 1));
            ImGui.Begin("HUD_BotomLeft", window_flags);
            {
                uint col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
                uint col_blue = ImGui.GetColorU32(new System.Numerics.Vector4(0.2f, 0.2f, 0.8f, 1));
                uint col_black = ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 1f));

                Imgui_Util.Progress_Bar_Stylised(CH_player.health / CH_player.health_max, new System.Numerics.Vector2(250, 15), col_red, col_black, 0.32f, 0.28f, 0.6f);
                ImGui.Spacing();
                Imgui_Util.Progress_Bar_Stylised(1f, new System.Numerics.Vector2(250, 15), col_blue, col_black, 0.32f, 0.28f, 0.6f);

                ImGui.Spacing();
                Imgui_Util.Title("{weapon image}");
                ImGui.SameLine();
                Imgui_Util.Title("10/10");
            }
            ImGui.End();

        }

        // blood overlay
        private float _boold_overlay_decreace_amout = 1.5f;
        private float _boold_overlay_intencity = 0.0f;
        public void flash_blood_overlay() { 
            
            if((CH_player.health / CH_player.health_max) <= 0.65f)
                _boold_overlay_intencity += 0.3f;
        }
        private void display_blood_overlay(float deltaTime) {

            float lower_limit = 1-((CH_player.health/ CH_player.health_max) * 2);
            Console.WriteLine($"lower_limit: {lower_limit}");
            if (lower_limit <= 0 && _boold_overlay_intencity <= 0)
                return;

            if(_boold_overlay_intencity > lower_limit)
                _boold_overlay_intencity -= deltaTime * _boold_overlay_decreace_amout;

            ImGuiWindowFlags window_flags_overlay = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;

            ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.GetColorU32(new System.Numerics.Vector4(0f, 0f, 0f, 0f)));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, _boold_overlay_intencity);

            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);
            ImGui.SetNextWindowSize(screen_size_buffer);
            ImGui.Begin("Screen_overlay", window_flags_overlay);
                ImGui.Image(image.Handle, screen_size_buffer);
            ImGui.End();

            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor();

        }

    }
}
