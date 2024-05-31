namespace Hell.enemy {

    using Core.physics;
    using Core.render;
    using Core.world;
    using Core.Controllers.ai;
    using OpenTK.Mathematics;

    public abstract class CH_base_NPC : Character {

        protected CH_base_NPC() {
            Add_Collider(new Collider(Collision_Shape.Circle));
            this.Set_Sprite(new Sprite());

            healthbar_slope = 0f;
            healthbar_width = 50;
            healthbar_height = 5;
            auto_remove_on_death = true;
        }

        public float damage;
        public int ray_number;
        public float ray_cast_range;
        public float ray_cast_angle;
        public float auto_detection_range;
        public float attack_range;
        public float fireDelay { get; set; } = 1f;
        public float DetectionRange { get; set; } = 400f;
        public AI_Controller Controller { get; set; } 

        protected float last_shoot_time = 0f;
        protected float shoot_interval;
        protected float lastFireTime { get; set; }

        public animation_data attack_anim;
        public animation_data walk_anim;
        public animation_data idle_anim;
        public animation_data current_anim;
        public animation_data hit_anim;

        public void set_animation_from_anim_data(animation_data animation_data) {
            if (current_anim.Equals(animation_data)) return;
            current_anim = animation_data;
            sprite.set_animation(
                animation_data.path_to_texture_atlas,
                animation_data.num_of_rows,
                animation_data.num_of_columns,
                animation_data.start_playing,
                animation_data.is_pixel_art,
                animation_data.fps,
                animation_data.loop);
        }

        public override void draw_imgui() {
            base.draw_imgui();

            if((health / health_max) < 1 && health > 0)
                Display_Healthbar(null, new System.Numerics.Vector2(-8,-40), new System.Numerics.Vector2(1), 5);
        }

        public static Vector2 RotateVector(Vector2 v, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
        }
 
        public abstract bool IsPlayerInRange();
        public abstract bool IsPlayerInAttackRange();
        public abstract bool IsHealthLow();
        public abstract void Move();
        public abstract void Pursue();
        public abstract void Attack();
        public abstract void Retreat();
    }
}