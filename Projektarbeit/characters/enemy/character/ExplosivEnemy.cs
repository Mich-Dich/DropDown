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

        public override void Attack()
        {
            if (CanFire())
            {
                Fire();
                CalculateAndApplySeparationForce();
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
    }
}
