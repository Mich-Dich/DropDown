
using Core.render;

namespace DropDown.enemy {

    internal class CH_orc : CH_base_NPC {

        public CH_orc() {

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
            damage = 25;
            auto_heal_amout = 5;
            health = 200;
            health_max = 200;
        }

        private void init_attack() {
            XP = 10;
            attack_range = 100;
            auto_detection_range = 600;
            ray_number = 15;
            ray_cast_range = 800;
            ray_cast_angle = float.Pi / 2;
        }

        private void init_animations() {
            attack_anim_notify_frame_index = 21;
            attack_anim = new animation_data("assets/animation/orc/attack_01.png", 8, 3, true, false, 30, true);
            walk_anim = new animation_data("assets/animation/orc/walk.png", 8, 4, true, false, 60, true);
            idle_anim = new animation_data("assets/animation/orc/idle_01.png", 16, 10, true, false, 30, true);
            death_anim = new animation_data("assets/animation/orc/death.png", 4, 3, true, false, 30, false);
        }
    }
}
