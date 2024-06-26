
using Core.render;

namespace DropDown.enemy {

    internal class CH_small_bug : CH_base_NPC {

        public CH_small_bug() {

            init_health();
            init_attack();
            init_animations();
            movement_speed = 450;
        }

        private void init_health() {
            damage = 10;
            auto_heal_amout = 2;
            health = 100;
            health_max = 100;
        }

        private void init_attack() {
            XP = 1;
            attack_range = 110;
            auto_detection_range = 500;
            ray_number = 12;
            ray_cast_range = 1000;
            ray_cast_angle = float.Pi / 2;
        }

        private void init_animations() {
            attack_anim = new animation_data("assets/animation/small_bug/attack_01.png", 8, 3, true, false, 30, true);
            walk_anim = new animation_data("assets/animation/small_bug/walk.png", 8, 4, true, false, 80, true);
            idle_anim = new animation_data("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 35, true);
            death_anim = new animation_data("assets/animation/small_bug/death.png", 4, 3, true, false, 30, false);
        }
    }
}
