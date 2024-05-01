﻿using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    internal class Hell : Core.game {

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        public sprite animation_sprite { get; set; }

        // ========================================================= functions =========================================================
        protected override void init() {

            set_update_frequency(144.0f);
            enable_debug_draw(true);

            this.player_controller = new PC_default();
            this.player = new player();
            this.active_map = new base_map();

            animation_sprite = new sprite(new Vector2(600, 200)).add_animation("assets/textures/Explosion-2", false, true, 30, true);
        }

        protected override void shutdown() { }

        protected override void update() { }

        protected override void render() {

            animation_sprite.draw();
        }

    }
}
