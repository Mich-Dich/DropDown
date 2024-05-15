﻿
namespace Hell.enemy {

    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class AIC_simple : AI_Controller {

        public AIC_simple(Character character)
            : base(character) {
            try {
                Set_Statup_State(typeof(EnterScreen));
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred in AIC_simple constructor: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }

    public class EnterScreen : I_AI_State {
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {
            character = (CH_base_NPC)aI_Controller.character;
            character.transform.position = new Vector2(-100, -100);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {
            character.execute_movement_pattern(Game_Time.delta);
            if(character.transform.position.X >= 0 && character.transform.position.Y >= 0)
                return typeof(MoveInPattern);
            return typeof(EnterScreen);
        }
    }

    public class MoveInPattern : I_AI_State {
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {
            character = (CH_base_NPC)aI_Controller.character;
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {
            character.execute_movement_pattern(Game_Time.delta);
            if((Game.Instance.player.transform.position - character.transform.position).LengthFast < character.attack_range)
                return typeof(Shoot);
            return typeof(MoveInPattern);
        }
    }

    public class Shoot : I_AI_State {
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {
            character = (CH_base_NPC)aI_Controller.character;
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {
            character.shoot_bullet_pattern();
            if((Game.Instance.player.transform.position - character.transform.position).LengthFast > character.attack_range)
                return typeof(MoveInPattern);
            if(character.ready_to_exit_screen())
                return typeof(ExitScreen);
            return typeof(Shoot);
        }
    }

    public class ExitScreen : I_AI_State {
        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {
            character = (CH_base_NPC)aI_Controller.character;
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {
            character.execute_exit_screen_movement(Game_Time.delta);
            if(character.transform.position.X < 0 || character.transform.position.Y < 0)
                return typeof(EnterScreen);
            return typeof(ExitScreen);
        }
    }

}
