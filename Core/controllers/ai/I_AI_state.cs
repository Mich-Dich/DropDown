
namespace Core.Controllers.ai {

    public interface I_AI_State {

        Type Execute(AI_Controller aiController);
        bool Exit(AI_Controller aiController);
        bool Enter(AI_Controller aiController);
    }
}
