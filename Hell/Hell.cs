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

            this.camera.set_min_max_zoom(0.5f, 3.0f);
            this.camera.set_zoom(1.2f);
            this.camera.set_position(new Vector2(450,350));

            this.active_map = new base_map();
            this.active_map.add_sprite(new sprite(new Vector2(600, 200)).add_animation("assets/textures/Explosion-2", true, false, 30, true));
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) { }

        protected override void render(float delta_time) { }

        protected override void render_imgui(float delta_time) { }
    }
}
