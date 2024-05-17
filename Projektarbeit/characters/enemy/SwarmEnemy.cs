namespace Hell.enemy {
    using OpenTK.Mathematics;
    using Core.render;
    using Core;
    using Core.util;

    public class SwarmEnemy : CH_base_NPC {

        private const float StopDistance = 350f; // Distance at which the enemy stops pursuing the player
        private const float PursueSpeed = 60; // Speed at which the enemy pursues the player

        public SwarmEnemy() : base() {
            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = float.Pi / 2;

            damage = 5;
            ray_number = 15;
            ray_cast_range = 800;
            ray_cast_angle = float.Pi/2;
            auto_detection_range = 100;
            attack_range = 50;

            last_shoot_time = 0f;
            shoot_interval = 0.4f;

            attack_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walk_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idle_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
        }

        public void Move() {
            Vector2 direction = new Vector2(0, -1);
            Random random = new Random();
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, 0);
            direction.NormalizeFast();
            direction *= movement_speed;
            movement_force = random.Next((int)movement_speed, (int)movement_speed_max);
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * movement_force * Game_Time.delta;
            if (velocity.Length() > movement_speed_max) {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }
            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);
        }

        public void Pursue() {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = playerPosition - transform.position;

            if (direction.Length < StopDistance) {
                // If the enemy is close enough to the player, stop moving
                return;
            }

            direction.NormalizeFast();
            direction *= PursueSpeed;

            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > movement_speed_max) {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);
        }

        public void Attack() {
            // Specific attack logic for SwarmEnemy
        }

        public void Die() {
            // Specific dying logic for SwarmEnemy
        }
    }
}