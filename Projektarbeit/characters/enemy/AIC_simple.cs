
namespace Hell.enemy {

    using Core.Controllers.ai;

    public class AIC_simple : AI_Swarm_Controller {

        public AIC_simple() => get_state_machine().Set_Statup_State(typeof(EnterScreen));

        public virtual bool Exit() { return true; }

        public virtual void Update(float delta_time) { }

        public virtual bool Enter() {

            if(characters.Count == 0) {
                Console.WriteLine("No characters to enter.");
                return false;
            }

            foreach(var character in characters) {
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);
            }
            return true;
        }

        public virtual Type? Execute() {

            if(characters.Count == 0) {
                Console.WriteLine("No characters to execute.");
                return null;
            }

            foreach(var character in characters) {
                Console.WriteLine($"position Length: {((CH_base_NPC)character).transform.position.LengthFast}");
            }
            return typeof(EnterScreen);
        }
    }
}
