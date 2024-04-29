namespace Projektarbeit
{
    public interface IState
    {
        void Handle(IContext context);
    }
}