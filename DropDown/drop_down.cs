﻿using Core.game_objects;
using Core.util;
using Core.visual;
using DropDown.player;
using Hell;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DropDown {

    internal class drop_down : Core.game {

        public drop_down(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        //private game_object test_cursor_object;
        //private game_object test_cursor_object_2;

        // ========================================================= functions =========================================================
        protected override void init() {
            
            GL.ClearColor(new Color4(.05f, .05f, .05f, 1f));
            set_update_frequency(144.0f);
            show_debug_data(true);

            this.player_controller = new PC_default();
            this.player = new CH_player();

            this.active_map = new base_map();

            this.camera.set_min_max_zoom(0.05f, 10.9f);
            
            //test_cursor_object = new game_object(new Vector2(150, 150)).set_sprite(resource_manager.get_texture("assets/defaults/default_grid_bright.png"));
            //this.active_map.add_game_object(test_cursor_object);

            //test_cursor_object_2 = new game_object(new Vector2(150, 150)).set_sprite(resource_manager.get_texture("assets/defaults/default_grid_bright.png"));
            //this.active_map.add_game_object(test_cursor_object_2);

            //this.active_map.add_sprite(new sprite(new Vector2(600, 200), new Vector2(500, 500)).add_animation("assets/textures/explosion", true, false, 60, true));
            //this.active_map.add_sprite(new sprite(new Vector2(-400, -200), new Vector2(300, 300)).add_animation("assets/textures/FX_explosion/animation_explosion.png", 8, 6, true, false, 60, true));
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) {

            //test_cursor_object.transform.position = this.camera.get_uper_left_screen_corner_in_world_coordinates();
            //test_cursor_object_2.transform.position = this.camera.get_lower_right_screen_corner_in_world_coordinates();
            
            //Console.WriteLine($"resultin pos: {test_cursor_object.transform.position}");
        }

        protected override void window_resize() {

            this.camera.set_zoom(((float)this.window.Size.X / 3500.0f) + this.camera.zoom_offset);
        }

        protected override void render(float delta_time) { }
        
        protected override void render_imgui(float delta_time) { }

    }
}
