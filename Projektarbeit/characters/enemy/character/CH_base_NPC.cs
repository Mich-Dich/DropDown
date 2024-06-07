namespace Projektarbeit.characters.enemy.character
{
    using Core.Controllers.ai;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public abstract class CH_base_NPC : Character
    {
        protected CH_base_NPC()
        {
            Add_Collider(new Collider(Collision_Shape.Circle));
            Set_Sprite(new Sprite());

            healthbar_slope = 0f;
            healthbar_width = 50;
            healthbar_height = 5;
            auto_remove_on_death = true;
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

        public Vector2 RotateVector(Vector2 v, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Vector2(
                (v.X * cos) - (v.Y * sin),
                (v.X * sin) + (v.Y * cos));
        }

        public void set_animation_from_anim_data(animation_data animation_data)
        {
            if (currentAnim.Equals(animation_data))
            {
                return;
            }

            currentAnim = animation_data;
            sprite.set_animation(
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

            if (health / health_max < 1 && health > 0)
            {
                Display_Healthbar(null, new System.Numerics.Vector2(-8, -40), new System.Numerics.Vector2(1), 5);
            }
        }

        public (Vector2, float) CalculateSeparationForce()
        {
            Random random = new();
            float separationDistance = 80f + ((float)random.NextDouble() * 30f);
            float separationSpeed = 15f + ((float)random.NextDouble() * 10f);
            float maxSeparationForce = 80f;

            Vector2 totalSeparationForce = Vector2.Zero;
            int nearbyCount = 0;

            foreach (var other in Controller.characters)
            {
                if (other == this)
                {
                    continue;
                }

                float distance = (other.transform.position - transform.position).Length;
                if (distance < separationDistance)
                {
                    Vector2 separationDirection = transform.position - other.transform.position;
                    separationDirection.NormalizeFast();

                    float separationForceMagnitude = (float)Math.Exp(-distance / 20f) * maxSeparationForce;
                    Vector2 separationForce = separationDirection * separationForceMagnitude;
                    totalSeparationForce += separationForce;
                    nearbyCount++;
                }
            }

            if (nearbyCount > 0)
            {
                totalSeparationForce /= nearbyCount;

                float jitterAngle = (float)(random.NextDouble() - 0.5f) * 0.5f;
                totalSeparationForce = RotateVector(totalSeparationForce, jitterAngle);
            }

            return (totalSeparationForce, separationSpeed);
        }

        public void CalculateAndApplySeparationForce()
        {
            var (totalSeparationForce, separationSpeed) = CalculateSeparationForce();

            var separationVelocity = new Box2DX.Common.Vec2(totalSeparationForce.X, totalSeparationForce.Y) * Game_Time.delta;

            if (separationVelocity.Length() > separationSpeed)
            {
                separationVelocity.Normalize();
                separationVelocity *= separationSpeed;
            }

            Add_Linear_Velocity(separationVelocity);
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