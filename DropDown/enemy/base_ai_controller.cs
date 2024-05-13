using Core.Controllers.ai;
using Core.world;

namespace DropDown.enemy {

    public class Base_AI_Controller : AI_Controller {

        public Base_AI_Controller(Character character) 
            : base(character) {
        
            

        }


    }

    public class Idle : I_AI_State {
    
        public Type Enter(AI_Controller aI_Controller) {
            throw new NotImplementedException();
        }

        public Type Execute(AI_Controller aI_Controller) {
            throw new NotImplementedException();
        }

        public Type Exit(AI_Controller aI_Controller) {
            throw new NotImplementedException();
        }
    }


}
