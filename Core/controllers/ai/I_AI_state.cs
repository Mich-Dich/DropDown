
namespace Core.controllers.ai {

    public interface I_AI_State {

        /// <summary>
        /// Executes internal logic for the AI controller and returns the next state Type to transition to.
        /// </summary>
        /// <param name="aiController">The AI controller instance that manages the state execution.</param>
        /// <returns>The Type of the next state to transition to (e.g., typeof(ThisState), typeof(WalkState), etc.).</returns>
        Type Execute(AI_Controller aiController);

        Type Exit(AI_Controller aiController);

        Type Enter(AI_Controller aiController);
    }
}
