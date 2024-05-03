
namespace Core.controllers.ai {

    public interface I_AI_state {

        /// <summary>
        /// Executes internal logic for the AI controller and returns the next state Type to transition to.
        /// </summary>
        /// <param name="ai_controller">The AI controller instance that manages the state execution.</param>
        /// <returns>The Type of the next state to transition to (e.g., typeof(ThisState), typeof(WalkState), etc.).</returns>
        Type execute(ai_controller ai_controller);

        Type exit(ai_controller ai_controller);

        Type enter(ai_controller ai_controller);
    }
}
