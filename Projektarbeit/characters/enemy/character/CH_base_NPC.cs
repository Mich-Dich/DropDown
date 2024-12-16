namespace Projektarbeit.characters.enemy.character
{
    using Core.Controllers.ai;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class CH_base_NPC : Character
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

        protected float StopDistance = 200f;
        protected float PursueSpeed = 60;
        private readonly Random random = new();

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

        public virtual bool IsPlayerInRange()
        {
            return IsPlayerInProximity(DetectionRange);
        }

        public virtual bool IsPlayerInAttackRange()
        {
            return IsPlayerInProximity(StopDistance);
        }

        public virtual bool IsHealthLow()
        {
            return health <= health_max * 0.2;
        }

        public virtual void Move()
        {
            Vector2 direction = GetRandomDirection();
            ApplyForceInDirection(direction, movement_speed);
        }

        public virtual void Pursue()
        {
            if (!IsPlayerInProximity(StopDistance))
            {
                Vector2 direction = GetDirectionToPlayer();
                ApplyForceInDirection(direction, PursueSpeed);
                CalculateAndApplySeparationForce();
            }
        }

        public virtual void Attack()
        {
            if (Game_Time.total - lastFireTime >= fireDelay + ((float)random.NextDouble() * (1f - 0.2f)) + 0.2f)
            {
                if (Game_Time.total - lastFireTime >= fireDelay)
                {
                    Vector2 direction = GetDirectionToPlayer();
                    Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(transform.position, direction));
                    lastFireTime = Game_Time.total;
                }

                CalculateAndApplySeparationForce();
            }
        }

        public virtual void Retreat()
        {
            Vector2 direction = GetDirectionAwayFromPlayer();
            ApplyForceInDirection(direction, movement_speed);
            CalculateAndApplySeparationForce();
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile projectile1 && projectile1.FiredByPlayer)
            {
                apply_damage(projectile1.Damage);
                set_animation_from_anim_data(hitAnim);
            }

            if (hit.hit_object is IReflectable projectile)
            {
                if (projectile.Reflected)
                {
                    apply_damage(projectile.Damage * 3);
                    set_animation_from_anim_data(hitAnim);
                }
            }

            if (hit.hit_object is Reflect reflect && collider != null && collider.body != null)
            {
                apply_damage(reflect.Damage);
                Box2DX.Common.Vec2 direction = new(transform.position.X - hit.hit_position.X, transform.position.Y - hit.hit_position.Y);
                if (collider != null && collider.body != null)
                {
                    collider.body.ApplyForce(direction * 100000f, collider.body.GetWorldCenter());
                }

                set_animation_from_anim_data(hitAnim);
            }
        }

        protected void ApplySeparation()
        {
            (Vector2 totalSeparationForce, float separationSpeed) = CalculateSeparationForce();

            if (totalSeparationForce.Length > 0)
            {
                Box2DX.Common.Vec2 separationVelocity =
                    new Box2DX.Common.Vec2(totalSeparationForce.X, totalSeparationForce.Y) * Game_Time.delta;

                if (separationVelocity.Length() > separationSpeed)
                {
                    separationVelocity.Normalize();
                    separationVelocity *= separationSpeed;
                }

                Add_Linear_Velocity(separationVelocity);
            }
        }

        private Vector2 GetRandomDirection()
        {
            float angle = (float)random.NextDouble() * MathF.PI * 2;
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        protected Vector2 GetDirectionToPlayer()
        {
            return (Game.Instance.player.transform.position - transform.position).Normalized();
        }

        protected Vector2 GetDirectionAwayFromPlayer()
        {
            return (transform.position - Game.Instance.player.transform.position).Normalized();
        }

        protected bool IsPlayerInProximity(float distance)
        {
            if (Game.Instance.player == null || Game.Instance.player.IsDead)
            {
                return false;
            }

            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - transform.position).Length;
            return distanceToPlayer <= distance;
        }

        protected void ApplyForceInDirection(Vector2 direction, float force)
        {
            direction.NormalizeFast();
            direction *= force;
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > movement_speed_max)
            {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
            //rotate_to_vector_smooth(direction);
        }

    }
}