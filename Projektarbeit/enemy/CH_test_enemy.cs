using Core.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hell.enemy {

    internal class CH_test_enemy : CH_base_NPC {

        public CH_test_enemy() {

            ray_number = 1;
            ray_cast_range = 1000;
            ray_cast_angle = float.Pi/2;
            auto_detection_range = 500;
            attack_range = 300;
            damage = 10;

            attack_anim = new animation_data("assets/animation/enemy/enemy.png", 1, 5, true, false, 10, true);
            walk_anim = new animation_data("assets/animation/enemy/enemy.png", 1, 5, true, false, 10, true);
            idle_anim = new animation_data("assets/animation/enemy/enemy.png", 1, 5, true, false, 10, true);
        }
    }
}
