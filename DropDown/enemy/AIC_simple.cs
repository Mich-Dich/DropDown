
namespace DropDown.enemy {

    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class AIC_simple : AI_Controller {

        public AIC_simple(Character character) 
            : base(character) {

            Set_Statup_State(typeof(idle));
            character.death_callback = () => { force_set_state(typeof(death)); };
        }
    }


    public class death : I_AI_State {

        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            Console.WriteLine($"DEATH");
            character = (CH_base_NPC)aI_Controller.character;
            character.set_animation_from_anim_data(character.death_anim);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) { return typeof(death); }
    }

    public class idle : I_AI_State {

        List<Game_Object> intersected_game_objects = new List<Game_Object>();
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            character = (CH_base_NPC)aI_Controller.character;
            character.set_animation_from_anim_data(character.idle_anim);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            intersected_game_objects.Clear();
            character.perception_check(ref intersected_game_objects, 0, character.ray_number, character.ray_cast_angle, character.ray_cast_range);
            if (intersected_game_objects.Contains(Game.Instance.player))
                return typeof(pursue_player);

            float player_distance = (Game.Instance.player.transform.position - character.transform.position).LengthFast;
            if(player_distance < character.auto_detection_range)
                return typeof(pursue_player);

            //if(Game.Instance.showDebug) 
            //    basic_drawer.Draw_Circle(character.transform.position, character.auto_detection_range);   
            
            return typeof(idle);
        }
    }

    public class pursue_player : I_AI_State {
        
        List<Game_Object> intersected_game_objects = new List<Game_Object>();
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            character = (CH_base_NPC)aI_Controller.character;
            character.set_animation_from_anim_data(character.walk_anim);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            Vector2 player_vec = Game.Instance.player.transform.position - character.transform.position;
            float player_distance = player_vec.LengthFast;

            intersected_game_objects.Clear();
            character.perception_check(ref intersected_game_objects, 0, character.ray_number, character.ray_cast_angle, character.ray_cast_range);
            if(!intersected_game_objects.Contains(Game.Instance.player)
                && player_distance > character.auto_detection_range)
                return typeof(idle);

            if(player_distance < character.attack_range)
                return typeof(attack_player);

            //if(Game.Instance.showDebug) 
            //    basic_drawer.Draw_Circle(character.transform.position, character.auto_detection_range);

            player_vec.NormalizeFast();
            character.add_force(new Box2DX.Common.Vec2(player_vec.X, player_vec.Y) * character.movement_force * Game_Time.delta);
            character.rotate_to_vector_smooth(player_vec);

            return typeof(pursue_player);
        }
    }


    public class attack_player : I_AI_State {
        
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            character = (CH_base_NPC)aI_Controller.character;
            character.set_animation_from_anim_data(character.attack_anim);
            character.sprite.animation.add_animation_notification(21, () => {

                var look_dir = util.vector_from_angle(character.transform.rotation - character.rotation_offset);
                Vector2 start = character.transform.position + (look_dir * (character.transform.size.X/2));
                Vector2 end = start + (look_dir * (character.attack_range - (character.transform.size.X/2)));

                if (Game.Instance.get_active_map().ray_cast(start, end, out Box2DX.Common.Vec2 intersection_point, out float distance, out Game_Object intersected_game_object, true, 0.5f))
                    intersected_game_object.Hit(new Core.physics.hitData(character.damage));
            });
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            Vector2 player_vec = Game.Instance.player.transform.position - character.transform.position;

            float player_distance = player_vec.LengthFast;
            if(player_distance > character.attack_range)
                return typeof(pursue_player);

            player_vec.NormalizeFast();
            character.rotate_to_vector_smooth(player_vec);

            return typeof(attack_player);
        }
    }

}
