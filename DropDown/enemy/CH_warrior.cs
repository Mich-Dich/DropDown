
using Core.render;

namespace DropDown.enemy {

    internal class CH_warrior : CH_base_NPC {

        public CH_warrior() {

            init_transform();
            init_health();
            init_attack();
            init_animations();
        }

        private void init_transform() {
            transform.size = new OpenTK.Mathematics.Vector2(160);
            movement_force = 80000000;
        }

        private void init_health() {
            damage = 20;
            auto_heal_amout = 4;
            health = 300;
            health_max = 300;
        }

        private void init_attack() {
            XP = 30;
            attack_range = 180;
            auto_detection_range = 500;
            ray_number = 15;
            ray_cast_range = 800;
            ray_cast_angle = float.Pi / 2;
        }

        private void init_animations() {
            attack_anim_notify_frame_index = 21;
            attack_anim = new animation_data("assets/animation/warrior/attack_01.png", 8, 3, true, false, 30, true);
            walk_anim = new animation_data("assets/animation/warrior/walk.png", 8, 4, true, false, 60, true);
            idle_anim = new animation_data("assets/animation/warrior/idle_01.png", 16, 10, true, false, 30, true);
            death_anim = new animation_data("assets/animation/warrior/death.png", 4, 3, true, false, 30, false);
        }
    }
}
