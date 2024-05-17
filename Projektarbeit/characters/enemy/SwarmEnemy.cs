namespace Hell.enemy {
    using OpenTK.Mathematics;
    using Core.render;
    using Core;
    using Core.util;

    public class SwarmEnemy : CH_base_NPC {

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

        public override void Move() {
            Vector2 direction = new Vector2(0, -1);
            Random random = new Random();
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, offset);
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

        public override void Attack() {
            // Specific attack logic for SwarmEnemy
        }

        public override void Die() {
            // Specific dying logic for SwarmEnemy
        }
    }
}