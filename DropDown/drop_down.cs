using Core.renderer;
using Core.util;
using Hell;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DropDown {

    internal class drop_down : Core.game {

        public drop_down(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        // ========================================================= functions =========================================================
        protected override void init() {

            set_update_frequency(144.0f);
            this.player_controller = new PC_default();

            this.default_map.generate_square(60, 30);

            this.player = new player();





        }

        protected override void shutdown() { }

        protected override void update() { }

        protected override void render() {

        }

    }
}
