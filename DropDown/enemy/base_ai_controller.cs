﻿
namespace DropDown.enemy {
    using Box2DX.Common;
    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class Base_AI_Controller : AI_Controller {

        public Base_AI_Controller(Character character) 
            : base(character) {

            Set_Statup_State(typeof(idle));
        }
    }


    public class idle : I_AI_State {
    
        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 30, true);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            float player_distance = (Game.Instance.player.transform.position - aI_Controller.character.transform.position).LengthFast;
            if(player_distance < 700)
                return typeof(pursue_player);

            return typeof(idle);
        }
    }


    public class pursue_player : I_AI_State {

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/walk.png", 8, 4, true, false, 80, true);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            Vector2 player_vec = Game.Instance.player.transform.position - aI_Controller.character.transform.position;

            float player_distance = player_vec.LengthFast;
            if(player_distance > 700)
                return typeof(idle);
            if(player_distance < 150)
                return typeof(attack_player);

            player_vec.NormalizeFast();
            aI_Controller.character.add_force(new Box2DX.Common.Vec2(player_vec.X, player_vec.Y) * aI_Controller.character.movement_force * Game_Time.delta);
            aI_Controller.character.rotate_to_vector_smooth(player_vec);

            return typeof(pursue_player);
        }
    }


    public class attack_player : I_AI_State {

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/attack_01.png", 8, 3, true, false, 30, true);
            aI_Controller.character.sprite.animation.add_animation_notification(21, () => {

                var look_dir = Util.vector_from_angle(aI_Controller.character.transform.rotation);
                Vector2 start = aI_Controller.character.transform.position + (look_dir * (aI_Controller.character.transform.size.X/2));
                Vector2 end = start + (look_dir * 150);

                Console.WriteLine($"start: {start}, end: {end}");

                bool hit = Game.Instance.get_active_map().ray_cast(start, end, out Vec2 intersection_point, out float distance, out Game_Object intersected_game_object);

                Console.WriteLine($"intersection_point: {intersection_point}, distance: {distance}, intersected_game_object: {intersected_game_object}");

                if (hit) {

                    intersected_game_object.Hit(new Core.physics.hitData(5.0f));
                    //Game.Instance.player.Hit(new Core.physics.hitData(5.0f));
                }


            });
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            Vector2 player_vec = Game.Instance.player.transform.position - aI_Controller.character.transform.position;

            float player_distance = player_vec.LengthFast;
            if(player_distance > 150)
                return typeof(pursue_player);

            player_vec.NormalizeFast();
            aI_Controller.character.rotate_to_vector_smooth(player_vec);

            return typeof(attack_player);
        }
    }

}
