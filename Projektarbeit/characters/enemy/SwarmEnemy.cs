namespace Hell.enemy {
    using OpenTK.Mathematics;
    using Core.render;

    public class SwarmEnemy : CH_base_NPC {

        public SwarmEnemy() : base() {
            transform.size = new Vector2(40);
            movement_speed = 500;
            movement_speed_max = 1000;
            movement_force = 5000000;
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
            // Specific movement logic for SwarmEnemy
            // For example, move in a sine wave pattern
        }

        public override void Attack() {
            // Specific attack logic for SwarmEnemy
        }

        public override void Die() {
            // Specific dying logic for SwarmEnemy
        }
    }
}