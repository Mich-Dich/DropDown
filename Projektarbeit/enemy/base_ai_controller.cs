
namespace Hell.enemy {
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
    
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.Add_Animation("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 30, true);
            return true;
        }

        public bool Exit(AI_Controller aI_Controller) { return true; }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            float player_distance = (Game.Instance.player.transform.position - aI_Controller.character.transform.position).LengthFast;
            if(player_distance < 700)
                return typeof(pursue_player);

            return typeof(idle);
        }
    }


    public class pursue_player : I_AI_State {

        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.Add_Animation("assets/animation/small_bug/walk.png", 8, 4, true, false, 80, true);
            return true;
        }

        public bool Exit(AI_Controller aI_Controller) { return true; }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            Vector2 player_vec = Game.Instance.player.transform.position - aI_Controller.character.transform.position;

            float player_distance = player_vec.LengthFast;
            if(player_distance > 700)
                return typeof(idle);
            if(player_distance < 150)
                return typeof(attack_player);

            player_vec.NormalizeFast();
            aI_Controller.character.add_force(new Vec2(player_vec.X, player_vec.Y) * aI_Controller.character.movement_force * Game_Time.delta);

            aI_Controller.character.rotate_to_vector(player_vec, 0.05f);


            return typeof(pursue_player);
        }
    }


    public class attack_player : I_AI_State {

        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.Add_Animation("assets/animation/small_bug/attack_01.png", 8, 3, true, false, 30, true);
            return true;
        }

        public bool Exit(AI_Controller aI_Controller) { return true; }

        public Type Execute(AI_Controller aI_Controller) {

            // look for player distance
            Vector2 player_vec = Game.Instance.player.transform.position - aI_Controller.character.transform.position;

            float player_distance = player_vec.LengthFast;
            if(player_distance > 150)
                return typeof(pursue_player);

            player_vec.NormalizeFast();
            //aI_Controller.character.add_force(new Vec2(player_vec.X, player_vec.Y) * aI_Controller.character.movement_force * Game_Time.delta);
            aI_Controller.character.rotate_to_vector(player_vec, 0.05f);


            return typeof(attack_player);
        }
    }

}


/*

*/