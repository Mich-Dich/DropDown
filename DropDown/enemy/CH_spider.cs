
using Core.render;

namespace DropDown.enemy {

    internal class CH_spider : CH_base_NPC {

        public CH_spider() {

            transform.size = new OpenTK.Mathematics.Vector2(160);
            ray_number = 15;
            ray_cast_range = 800;
            ray_cast_angle = float.Pi / 2;
            auto_detection_range = 400;
            attack_range = 150;
            movement_force = 80000000;
            damage = 15;

            auto_heal_amout = 2;
            health = 160;
            health_max = 160;
            
            attack_anim_notify_frame_index = 21;
            attack_anim = new animation_data("assets/animation/spider/attack_01.png", 8, 3, true, false, 30, true);
            walk_anim = new animation_data("assets/animation/spider/walk.png", 8, 4, true, false, 60, true);
            idle_anim = new animation_data("assets/animation/spider/idle_01.png", 16, 10, true, false, 30, true);
            death_anim = new animation_data("assets/animation/spider/death.png", 4, 3, true, false, 30, false);
        }

    }
}
