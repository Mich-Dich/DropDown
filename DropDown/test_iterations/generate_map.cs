using Core;
using Core.game_objects;
using Core.manager;
using Core.renderer;
using Core.util;
using Core.visual;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DropDown {

    internal class generate_map : Core.game {

        public generate_map(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        // ========================================================= functions =========================================================
        protected override void init() {

            this.player_controller = new PC_default();
            set_update_frequency(144.0f);
        }

        protected override void shutdown() { }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

        }

    }
}
