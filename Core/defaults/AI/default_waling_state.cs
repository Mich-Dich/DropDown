using Core.controllers.ai;

namespace Core.defaults.AI {

    public class default_waling_state : I_AI_state {


        Type I_AI_state.enter(ai_controller ai_controller) {
            throw new NotImplementedException();
        }

        Type I_AI_state.exit(ai_controller ai_controller) {
            throw new NotImplementedException();
        }

        Type I_AI_state.execute(ai_controller ai_controller) {


            // proccess internal logic and set next state to execute


            return typeof(default_waling_state); // return calass of state to transition to
        }
    }
}
