namespace Projektarbeit.characters.enemy.character
{
    using System;
    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class SwarmEnemy : CH_base_NPC
    {
        private const float StopDistance = 200f;
        private const float PursueSpeed = 60;
        private readonly Random random = new();

        public SwarmEnemy()
            : base()
        {
            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = MathF.PI / 2;

            string enemyAnimationPath = "assets/animation/enemy/enemy.png";
            attackAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            walkAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            idleAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
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
