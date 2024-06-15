namespace Projektarbeit.characters.enemy.character
{
    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class TankEnemy : SwarmEnemy
    {
        private const float StopDistance = 200f;
        private const float PursueSpeed = 30;
        private readonly Random random = new();

        public TankEnemy()
            : base()
        {
            health_max = 300;
            health = health_max;

            transform.size = new Vector2(150);
            movement_speed = 10;
            movement_speed_max = 15;
            rotation_offset = float.Pi / 2;

            damage = 5;
            rayNumber = 15;
            rayCastRange = 800;
            rayCastAngle = float.Pi / 2;
            autoDetectionRange = 100;
            attackRange = 50;

            lastShootTime = 0f;
            shootInterval = 0.4f;
            fireDelay = 2f;

            attackAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walkAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public override bool IsPlayerInRange()
        {
            return IsPlayerAlive() && GetDistanceToPlayer() <= DetectionRange;
        }

        public override bool IsPlayerInAttackRange()
        {
            return IsPlayerAlive() && GetDistanceToPlayer() <= StopDistance;
        }

        public override bool IsHealthLow()
        {
            return health <= health_max * 0.2;
        }

        public override void Move()
        {
            Vector2 direction = GetRandomDirection();
            ApplyForceInDirection(direction, movement_speed);
        }

        public override void Pursue()
        {
            Vector2 direction = GetDirectionToPlayer();

            if (direction.Length < StopDistance)
            {
                return;
            }

            ApplyForceInDirection(direction, PursueSpeed);
            ApplySeparation();
        }

        public override void Attack()
        {
            if (Game_Time.total - lastFireTime >= fireDelay)
            {
                Vector2 direction = GetDirectionToPlayer();
                Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(transform.position, direction));
                lastFireTime = Game_Time.total;
            }

            ApplySeparation();
        }

        public override void Retreat()
        {
            Vector2 direction = GetDirectionAwayFromPlayer();
            ApplyForceInDirection(direction, movement_speed);
            ApplySeparation();
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

        private void ApplySeparation()
        {
            (Vector2 totalSeparationForce, float separationSpeed) = CalculateSeparationForce();

            Box2DX.Common.Vec2 separationVelocity =
                new Box2DX.Common.Vec2(totalSeparationForce.X, totalSeparationForce.Y) * Game_Time.delta;

            if (separationVelocity.Length() > separationSpeed)
            {
                separationVelocity.Normalize();
                separationVelocity *= separationSpeed;
            }

            Add_Linear_Velocity(separationVelocity);
        }

        private bool IsPlayerAlive()
        {
            return Game.Instance.player != null && !Game.Instance.player.IsDead;
        }

        private float GetDistanceToPlayer()
        {
            return (Game.Instance.player.transform.position - transform.position).Length;
        }

        private Vector2 GetRandomDirection()
        {
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            Vector2 direction = new Vector2(offset, 1);
            direction.NormalizeFast();
            return direction;
        }

        private Vector2 GetDirectionToPlayer()
        {
            return (Game.Instance.player.transform.position - transform.position).Normalized();
        }

        private Vector2 GetDirectionAwayFromPlayer()
        {
            return (transform.position - Game.Instance.player.transform.position).Normalized();
        }

        private void ApplyForceInDirection(Vector2 direction, float speed)
        {
            direction *= speed;
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > movement_speed_max)
            {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);
        }
    }
}