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

            StopDistance = 200f;
            PursueSpeed = 30;

            lastShootTime = 0f;
            shootInterval = 0.4f;
            fireDelay = 2f;

            attackAnim = new animation_data("assets/animation/enemy/tank.png", 5, 1, true, true, 10, true);
            walkAnim = new animation_data("assets/animation/enemy/tank.png", 5, 1, true, true, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/tank.png", 5, 1, true, true, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/tank-hit.png", 5, 1, true, true, 10, true);
        }
    }
}