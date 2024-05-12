
namespace Projektarbeit {

    using Projektarbeit.player;
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    internal class projektarbeit : Core.game {

        public projektarbeit(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private CH_player CH_player;

        // ========================================================= functions =========================================================
        protected override void init() {
            
            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            set_update_frequency(144.0f);
            show_debug_data(true);

            this.player_controller = new PC_default();

            CH_player = new CH_player();
            this.player = CH_player;

            this.active_map = new base_map();
            this.active_map.set_background_image("assets/textures/background/background.png");

            this.camera.set_min_max_zoom(0.05f, 10.9f);
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) { }

        protected override void window_resize() {

            this.camera.set_zoom(((float)this.window.Size.X / 3500.0f) + this.camera.zoom_offset);
        }

        protected override void render(float delta_time) { }
        
        protected override void render_imgui(float delta_time) {
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
