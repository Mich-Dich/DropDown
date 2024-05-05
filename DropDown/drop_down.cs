using DropDown.player;
using Hell;

namespace DropDown {

    internal class drop_down : Core.game {

        public drop_down(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }


        // ========================================================= functions =========================================================
        protected override void init() {

            set_update_frequency(144.0f);
            shoe_debug_data(true);

            this.player_controller = new PC_default();
            this.player = new CH_player();


            this.active_map = new base_map();
            this.camera.set_min_max_zoom(0.35f, 0.9f);

            //var ai_controller = new AI_default();
            //ai_controller.register_state(new List<Type> { typeof(default_waling_state) });

            //this.active_map.add_sprite(new sprite(new Vector2(600, 200), new Vector2(500, 500)).add_animation("assets/textures/explosion", true, false, 60, true));
            //this.active_map.add_sprite(new sprite(new Vector2(-400, -200), new Vector2(300, 300)).add_animation("assets/textures/FX_explosion/animation_explosion.png", 8, 6, true, false, 60, true));
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) { }

        protected override void render(float delta_time) { }
        
        protected override void render_imgui(float delta_time) { }

    }
}
