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

        public override void Pursue()
        {
            if (!ShouldPursuePlayer()) return;

            Vector2 direction = GetDirectionToTarget();
            ApplyForceInDirection(direction, PursueSpeed);
            //rotate_to_vector_smooth(direction);

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

        private void InitializeCharacter()
        {
            transform.size = new Vector2(60);
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

            StopDistance = 450f;
            PursueSpeed = 60;

            lastShootTime = 0f;
            shootInterval = 0.4f;
            fireDelay = 10f;

            attackAnim = new animation_data("assets/animation/enemy/sniper.png", 5, 1, true, false, 10, true);
            walkAnim = new animation_data("assets/animation/enemy/sniper.png", 5, 1, true, false, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/sniper.png", 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/sniper-hit.png", 5, 1, true, false, 10, true);
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
    }
}
