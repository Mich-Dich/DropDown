
using Core.physics;
using Core.render;
using Core.util;
using Core.world;
using Core;
using Box2DX.Common;
using Hell.weapon;
using OpenTK.Mathematics;

namespace Hell.enemy {

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

        private float last_shoot_time = 0f;
        private float shoot_interval = 0.4f; // Interval in seconds

        public animation_data attack_anim = new animation_data("assets/animation/enemy/enemy.png", 1, 5, true, false, 10, true);
        public animation_data walk_anim = new animation_data("assets/animation/enemy/enemy.png", 1, 5, true, false, 10, true);
        public animation_data idle_anim = new animation_data("assets/animation/enemy/enemy.png", 1, 5, true, false, 10, true);

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

        public virtual void shoot_bullet_pattern() {
            if(Game_Time.total - last_shoot_time >= shoot_interval) {
                Vector2 npcLocation = this.transform.position;
                Vec2 npcDirectionVec2 = this.collider.body.GetLinearVelocity();
                npcDirectionVec2.Normalize();
                Vector2 npcDirection = new Vector2(npcDirectionVec2.X, npcDirectionVec2.Y);
                Game.Instance.get_active_map().Add_Game_Object(new TestProjectile(npcLocation, npcDirection));
                last_shoot_time = Game_Time.total;
            }
        }

        public virtual void execute_movement_pattern(float deltaTime) {
            float radius = 100;
            float speed = 1;

            float newAngle = (speed * deltaTime) % (2 * MathF.PI);

            Vector2 newPosition = new Vector2(
                this.transform.position.X + radius * MathF.Cos(newAngle),
                this.transform.position.Y + radius * MathF.Sin(newAngle)
            );

            this.transform.position = newPosition;
        }

        public virtual bool ready_to_exit_screen() {
            return this.health <= 0;
        }

        public virtual void execute_exit_screen_movement(float deltaTime) {
            this.transform.position = new Vector2(this.transform.position.X - this.movement_speed * deltaTime, this.transform.position.Y);
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

        public animation_data(String path_to_texture_atlas, Int32 num_of_rows, Int32 num_of_columns, Boolean start_playing, Boolean is_pixel_art, Int32 fps, Boolean loop) {

            this.path_to_texture_atlas = path_to_texture_atlas;
            this.num_of_rows = num_of_rows;
            this.num_of_columns = num_of_columns;
            this.start_playing = start_playing;
            this.is_pixel_art = is_pixel_art;
            this.fps = fps;
            this.loop = loop;
        }
    }

}
