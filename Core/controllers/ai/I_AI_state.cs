
namespace Core.controllers.ai {

    public interface I_AI_state {

        // execute only logic, and return the next state to be in (eg. this, walk_state, ... )
        I_AI_state execute_state(ai_controller ai_controller);
    }
}
