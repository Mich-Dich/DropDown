namespace Core.Controllers.ai
{
    public interface I_AI_State
    {
        Type Execute(AI_Controller aiController);

        Type Exit(AI_Controller aiController);

        Type Enter(AI_Controller aiController);
    }
}
