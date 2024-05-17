namespace Hell.enemy {

    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public abstract class CH_base_NPC : Character {

        protected CH_base_NPC() {
            Add_Collider(new Collider(Collision_Shape.Circle));
            this.Set_Sprite(new Sprite());
        }

        public float damage;
        public int ray_number;
        public float ray_cast_range;
        public float ray_cast_angle;
        public float auto_detection_range;
        public float attack_range;

        protected float last_shoot_time;
        protected float shoot_interval;

        public animation_data attack_anim;
        public animation_data walk_anim;
        public animation_data idle_anim;

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
        }
    }
}