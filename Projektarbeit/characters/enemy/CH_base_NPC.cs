namespace Hell.enemy
{
    using Core.Controllers.ai;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public abstract class CH_base_NPC : Character
    {
        protected CH_base_NPC()
        {
            this.Add_Collider(new Collider(Collision_Shape.Circle));
            this.Set_Sprite(new Sprite());

            this.healthbar_slope = 0f;
            this.healthbar_width = 50;
            this.healthbar_height = 5;
            this.auto_remove_on_death = true;
        }

        public float damage;
        public int rayNumber;
        public float rayCastRange;
        public float rayCastAngle;
        public float autoDetectionRange;
        public float attackRange;

        public float DetectionRange { get; set; } = 400f;

        public AI_Controller Controller { get; set; }

        public animation_data attackAnim;
        public animation_data walkAnim;
        public animation_data idleAnim;
        public animation_data currentAnim;
        public animation_data hitAnim;

        protected float lastShootTime = 0f;
        protected float shootInterval;

        public static Vector2 RotateVector(Vector2 v, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Vector2(
                (v.X * cos) - (v.Y * sin),
                (v.X * sin) + (v.Y * cos));
        }

        public void set_animation_from_anim_data(animation_data animation_data)
        {
            if (this.currentAnim.Equals(animation_data))
            {
                return;
            }

            this.currentAnim = animation_data;
            this.sprite.set_animation(
                animation_data.path_to_texture_atlas,
                animation_data.num_of_rows,
                animation_data.num_of_columns,
                animation_data.start_playing,
                animation_data.is_pixel_art,
                animation_data.fps,
                animation_data.loop);
        }

        public override void draw_imgui()
        {
            base.draw_imgui();

            if ((this.health / this.health_max) < 1 && this.health > 0)
            {
                this.Display_Healthbar(null, new System.Numerics.Vector2(-8, -40), new System.Numerics.Vector2(1), 5);
            }
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