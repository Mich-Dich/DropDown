namespace Projektarbeit.characters.enemy.character
{
    using System;
    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class SniperEnemy : SwarmEnemy
    {
        private const float StopDistance = 450f;
        private const float PursueSpeed = 60;
        private const float PursueThreshold = 350f;
        private const float IdealDistanceFromPlayer = 450f;
        private const float DistanceTolerance = 100f;
        private const float SeparationDistance = 80f;
        private const float SeparationSpeed = 15f;
        private const float MaxSeparationForce = 80f;

        private Vector2 targetPosition;

        public SniperEnemy()
            : base()
        {
            InitializeCharacter();
            targetPosition = Game.Instance.player.transform.position;
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
            ApplyForceInDirection(direction);
            RotateToDirection(direction);
        }

        public override void Pursue()
        {
            if (!ShouldPursuePlayer()) return;

            Vector2 direction = GetDirectionToTarget();
            ApplyForceInDirection(direction);
            RotateToDirection(direction);

            ApplySeparation();
        }

        public override void Attack()
        {
            if (CanShoot())
            {
                ShootAtPlayer();
            }

            ApplySeparation();
        }

        public override void Retreat()
        {
            Vector2 direction = GetDirectionAwayFromPlayer();
            ApplyForceInDirection(direction);
            RotateToDirection(direction);

            ApplySeparation();
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile projectile1 && projectile1.FiredByPlayer)
            {
                ApplyDamageFromProjectile(projectile1);
            }

            if (hit.hit_object is IReflectable projectile)
            {
                if (projectile.Reflected)
                {
                    ApplyDamageFromReflectedProjectile(projectile);
                }
            }

            if (hit.hit_object is Reflect reflect && collider != null && collider.body != null)
            {
                ApplyDamageFromReflect(reflect, hit);
            }
        }

        private void InitializeCharacter()
        {
            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = float.Pi / 2;

            damage = 5;
            rayNumber = 15;
            rayCastRange = 800;
            rayCastAngle = float.Pi / 2;
            autoDetectionRange = 100;
            attackRange = 50;
            DetectionRange = 8000f;

            lastShootTime = 0f;
            shootInterval = 0.4f;
            fireDelay = 10f;

            attackAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walkAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        private void ApplySeparation()
        {
            (Vector2 totalSeparationForce, float separationSpeed) = CalculateSeparationForce();

            if (totalSeparationForce != Vector2.Zero)
            {
                ApplyForceInDirection(totalSeparationForce);
            }
        }

        private bool IsPlayerAlive()
        {
            return Game.Instance.player != null && !Game.Instance.player.IsDead;
        }

        private float GetDistanceToPlayer()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            return (playerPosition - transform.position).Length;
        }

        private Vector2 GetRandomDirection()
        {
            Vector2 direction = new(0, 1);
            Random random = new();
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, 0);
            direction.NormalizeFast();
            return direction;
        }

        private void ApplyForceInDirection(Vector2 direction)
        {
            direction *= movement_speed;
            movement_force = new Random().Next((int)movement_speed, (int)movement_speed_max);
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * movement_force * Game_Time.delta;
            if (velocity.Length() > movement_speed_max)
            {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
        }

        private void RotateToDirection(Vector2 direction)
        {
            rotate_to_vector_smooth(direction);
        }

        private bool ShouldPursuePlayer()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 toPlayer = playerPosition - transform.position;

            Vector2 desiredDirection = toPlayer.Normalized() * IdealDistanceFromPlayer;
            targetPosition = playerPosition - desiredDirection;

            float distanceToTarget = (targetPosition - transform.position).Length;
            if (Math.Abs(distanceToTarget - IdealDistanceFromPlayer) <= DistanceTolerance)
            {
                if (toPlayer.Length > PursueThreshold)
                {
                    targetPosition = playerPosition - desiredDirection;
                }
                else
                {
                    return false;
                }
            }

            Vector2 direction = targetPosition - transform.position;

            return direction.Length >= StopDistance;
        }

        private Vector2 GetDirectionToTarget()
        {
            Vector2 direction = targetPosition - transform.position;
            direction.NormalizeFast();
            return direction * PursueSpeed;
        }

        private bool CanShoot()
        {
            return Game_Time.total - lastFireTime >= fireDelay;
        }

        private void ShootAtPlayer()
        {
            Vector2 enemyLocation = transform.position;
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = (playerPosition - enemyLocation).Normalized();
            Game.Instance.get_active_map().Add_Game_Object(new SniperProjectile(enemyLocation, direction));
            lastFireTime = Game_Time.total;
        }

        private Vector2 GetDirectionAwayFromPlayer()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = transform.position - playerPosition;
            direction.NormalizeFast();
            return direction * movement_speed;
        }

        private void ApplyDamageFromProjectile(IProjectile projectile)
        {
            apply_damage(projectile.Damage);
            set_animation_from_anim_data(hitAnim);
        }

        private void ApplyDamageFromReflectedProjectile(IReflectable projectile)
        {
            apply_damage(projectile.Damage * 3);
            set_animation_from_anim_data(hitAnim);
        }

        private void ApplyDamageFromReflect(Reflect reflect, hitData hit)
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
}
