
namespace Hell.enemy {

    using Core.Controllers.ai;
    using Core.world;

    public class AIC_simple : AI_Controller {

        protected CH_base_NPC character;

        public AIC_simple(Character character)
            : base(character) {
            this.character = (CH_base_NPC)character;
            Set_Statup_State(typeof(EnterScreen));
        }

        public virtual bool Exit() { return true; }
        public virtual bool Enter() {
            character.set_animation_from_anim_data(character.idle_anim);
            return true;
        }

        public virtual Type Execute() {
            Console.WriteLine($"position Length: {character.transform.position.LengthFast}");
            return typeof(EnterScreen);
        }
    }

    public class EnterScreen : I_AI_State {

        CH_base_NPC character;

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            character = (CH_base_NPC)aI_Controller.character;
            character.set_animation_from_anim_data(character.idle_anim);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            Console.WriteLine($"position Length: {character.transform.position.LengthFast}");
            return typeof(EnterScreen);
        }
    }
}
