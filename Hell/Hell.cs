using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    internal class Hell : Core.game {

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        // ========================================================= functions =========================================================
        protected override void init() {
            set_update_frequency(144.0f);
            show_debug_data(true);

            this.player_controller = new PC_default();
            this.player = new player();

            this.active_map = new base_map();
            
            this.camera.set_position(new Vector2(500, 275));
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) { }

        protected override void window_resize() {

            this.camera.set_zoom((float)this.window.Size.X / 1200.0f);
        }

        protected override void render(float delta_time) { }

        protected override void render_imgui(float delta_time) { }
    }
}