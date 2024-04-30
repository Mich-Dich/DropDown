
namespace Core {
 
    public interface IState {
    
        void Handle(IContext context);
    }
}
