using Core.game_objects;

namespace Projektarbeit
{
    public interface IContext
    {
        IState State { get; set; }

        IPattern Pattern { get; set; }
        transform Transform { get; }
               
        void Update();
    }
}