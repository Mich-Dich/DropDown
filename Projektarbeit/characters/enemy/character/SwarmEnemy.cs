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
        
        

        public SwarmEnemy()
            : base()
        {
            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = MathF.PI / 2;

            StopDistance = 200f;
            PursueSpeed = 60;

            string enemyAnimationPath = "assets/animation/enemy/enemy.png";
            attackAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            walkAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            idleAnim = new animation_data(enemyAnimationPath, 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        
    }
}
