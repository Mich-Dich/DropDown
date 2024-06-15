namespace Projektarbeit.characters.enemy.character
{
    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class ExplosivEnemy : SwarmEnemy
    {
        private const float StopDistance = 400f;
        private const float PursueSpeed = 60;
        private const float HealthThreshold = 0.2f;
        private const float SeparationDistance = 80f;
        private const float SeparationSpeed = 15f;
        private const float MaxSeparationForce = 80f;

        private readonly Random random = new();

        public ExplosivEnemy()
            : base()
        {
            InitializeCharacter();
            InitializeAnimations();
        }

        public override bool IsPlayerInRange() => IsPlayerInProximity(DetectionRange);

        public override bool IsPlayerInAttackRange() => IsPlayerInProximity(StopDistance);

        public override bool IsHealthLow() => health <= health_max * HealthThreshold;

        public override void Move() => ApplyForceInDirection(GetRandomDirection(), movement_speed);

        public override void Pursue()
        {
            if (Game.Instance.player is null || Game.Instance.player.IsDead)
            {
                return;
            }

            var direction = GetDirectionToPlayer();

            if (direction.Length < StopDistance)
            {
                return;
            }

            ApplyForceInDirection(direction, PursueSpeed);
            CalculateAndApplySeparationForce();
        }

        public override void Attack()
        {
            if (CanFire())
            {
                Fire();
                CalculateAndApplySeparationForce();
            }
        }

        public override void Retreat()
        {
            if (Game.Instance.player is null || Game.Instance.player.IsDead)
            {
                return;
            }

            var direction = GetDirectionFromPlayer();
            ApplyForceInDirection(direction, movement_speed);
            CalculateAndApplySeparationForce();
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile projectile && projectile.FiredByPlayer)
            {
                ApplyProjectileDamage(projectile);
            }

            if (hit.hit_object is IReflectable reflectableProjectile && reflectableProjectile.Reflected)
            {
                ApplyReflectedProjectileDamage(reflectableProjectile);
            }

            if (hit.hit_object is Reflect reflect)
            {
                ApplyReflectDamage(reflect, hit);
            }
        }

        private void InitializeCharacter()
        {
            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = MathF.PI / 2;
            fireDelay = 10f;
        }

        private void InitializeAnimations()
        {
            var enemyAnimationPath = "assets/animation/enemy/enemy.png";
            var enemyHitAnimationPath = "assets/animation/enemy/enemy-hit.png";

            attackAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            walkAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            idleAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            hitAnim = new animation_data(enemyHitAnimationPath, 5, 1, true, false, 10, true);
        }

        private bool IsPlayerInProximity(float proximity)
        {
            if (Game.Instance.player is null || Game.Instance.player.IsDead)
            {
                return false;
            }

            var distanceToPlayer = GetDistanceToPlayer();
            return distanceToPlayer <= proximity;
        }

        private Vector2 GetRandomDirection()
        {
            var direction = new Vector2(0, 1);
            var offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, 0);
            direction.NormalizeFast();
            return direction;
        }

        private void ApplyForceInDirection(Vector2 direction, float speed)
        {
            direction *= speed;
            var velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;

            if (velocity.Length() > movement_speed_max)
            {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);
        }

        private Vector2 GetDirectionToPlayer() => Game.Instance.player.transform.position - transform.position;

        private Vector2 GetDirectionFromPlayer() => transform.position - Game.Instance.player.transform.position;

        private float GetDistanceToPlayer() => (Game.Instance.player.transform.position - transform.position).Length;

        private bool CanFire() => Game_Time.total - lastFireTime >= fireDelay + ((float)random.NextDouble() * (1f - 0.2f)) + 0.2f;

        private void Fire()
        {
            if (Game_Time.total - lastFireTime >= fireDelay)
            {
                var enemyLocation = transform.position;
                var direction = GetDirectionToPlayer().Normalized();
                Game.Instance.get_active_map().Add_Game_Object(new ExplosivProjectile(enemyLocation, direction));
                lastFireTime = Game_Time.total;
            }
        }

        private void ApplyProjectileDamage(IProjectile projectile)
        {
            apply_damage(projectile.Damage);
            set_animation_from_anim_data(hitAnim);
        }

        private void ApplyReflectedProjectileDamage(IReflectable projectile)
        {
            apply_damage(projectile.Damage * 3);
            set_animation_from_anim_data(hitAnim);
        }

        private void ApplyReflectDamage(Reflect reflect, hitData hit)
        {
            apply_damage(reflect.Damage);
            var direction = new Box2DX.Common.Vec2(transform.position.X - hit.hit_position.X, transform.position.Y - hit.hit_position.Y);
            collider?.body?.ApplyForce(direction * 100000f, collider.body.GetWorldCenter());
            set_animation_from_anim_data(hitAnim);
        }
    }
}
