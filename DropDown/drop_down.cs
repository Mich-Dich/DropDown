using Core.visual;
using OpenTK.Mathematics;

namespace DropDown {

    internal class drop_down : Core.game {

        public drop_down(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        public sprite animation_sprite { get; set; }

        // ========================================================= functions =========================================================
        protected override void init() {

            set_update_frequency(144.0f);
            this.player_controller = new PC_default();
            this.player = new player();

            this.default_map.generate_square(4, 3);
            
            animation_sprite = new sprite(new Vector2(600, 200)).set_animation("assets/textures/Explosion-2", false, true, 30, true);
        }

        protected override void shutdown() { }

        protected override void update() { }

        protected override void render() {

            animation_sprite.draw();
        }

    }
}
