using Core.game_objects;

namespace Core {

    public interface IContext {

        IState State { get; set; }
        IPattern Pattern { get; set; }
        transform Transform { get; }
               
        void Update();
    }
}
