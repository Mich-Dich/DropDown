using Core.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropDown.enemy {

    internal class CH_small_bug : CH_base_NPC {

        public CH_small_bug() {

            ray_number = 1;
            ray_cast_range = 1000;
            ray_cast_angle = float.Pi/2;
            auto_detection_range = 500;
            attack_range = 300;
            damage = 10;

            attack_anim = new animation_data("assets/animation/small_bug/attack_01.png", 8, 3, true, false, 30, true);
            walk_anim = new animation_data("assets/animation/small_bug/walk.png", 8, 4, true, false, 80, true);
            idle_anim = new animation_data("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 30, true);
        }

        public override void Draw() {
            base.Draw();

            Display_Healthbar();
        }

    }
}
