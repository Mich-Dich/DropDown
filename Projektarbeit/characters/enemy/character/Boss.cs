namespace Projektarbeit.characters.enemy.character
{
    using Core.Controllers.ai;
    using Core.render;
    using OpenTK.Mathematics;

    public class Boss : CH_base_NPC
    {
        public Boss(AI_Controller controller)
            : base()
        {
            Controller = controller;
            transform.size = new Vector2(500);
            health_max = 500;
            health = health_max;

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

            attackAnim = new animation_data("assets/animation/enemy/CrystalKnightAttack.png", 5, 1, true, true, 5, false);
            walkAnim = new animation_data("assets/animation/enemy/CrystalKnightIdle.png", 4, 1, true, true, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/CrystalKnightIdle.png", 4, 1, true, true, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/CrystalKnightHit.png", 1, 1, true, true, 2, false);
        }
    }
}