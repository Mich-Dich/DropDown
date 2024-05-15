
using Core.physics;
using Core.render;
using Core.util;
using Core.world;
using OpenTK.Mathematics;

namespace DropDown.enemy {

    public class CH_base_NPC : Character{

        public CH_base_NPC() {

            transform.size = new Vector2(80);
            movement_speed = 500;
            movement_speed_max = 1000;
            movement_force = 5000000;
            rotation_offset = float.Pi/2;

            Add_Collider(new Collider(Collision_Shape.Circle));

            this.Set_Sprite(new Sprite());

        }

        public float damage = 15;
        public int ray_number = 15;
        public float ray_cast_range = 800;
        public float ray_cast_angle = float.Pi/2;
        public float auto_detection_range = 400;
        public float attack_range = 150;

        public int attack_anim_notify_frame_index = 21;
        public animation_data attack_anim = new animation_data("assets/animation/small_bug/attack_01.png", 8, 3, true, false, 30, true);
        public animation_data walk_anim = new animation_data("assets/animation/small_bug/walk.png", 8, 4, true, false, 80, true);
        public animation_data idle_anim = new animation_data("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 30, true);
        
        public void set_animation_from_anim_data(animation_data animation_data) {

            sprite.set_animation(
                animation_data.path_to_texture_atlas,
                animation_data.num_of_rows,
                animation_data.num_of_columns, 
                animation_data.start_playing, 
                animation_data.is_pixel_art,
                animation_data.fps,
                animation_data.loop);
        }

    }

    public struct animation_data {

        public string path_to_texture_atlas = "assets/defaults/default_grid.png";
        public int num_of_rows = 1;
        public int num_of_columns = 1;
        public bool start_playing = false;
        public bool is_pixel_art = false;
        public int fps = 30;
        public bool loop = false;

        public animation_data(String path_to_texture_atlas, Int32 num_of_colums, Int32 num_of_rows, Boolean start_playing, Boolean is_pixel_art, Int32 fps, Boolean loop) {

            this.path_to_texture_atlas = path_to_texture_atlas;
            this.num_of_rows = num_of_colums;
            this.num_of_columns = num_of_rows;
            this.start_playing = start_playing;
            this.is_pixel_art = is_pixel_art;
            this.fps = fps;
            this.loop = loop;
        }
    }

}
