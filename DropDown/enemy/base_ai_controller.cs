using Core.controllers.ai;

namespace DropDown.enemy {

    public class base_ai_controller : ai_controller {

        public base_ai_controller() {
        
            

        }


    }


    public class idle : I_AI_state {
    
        public Type enter(ai_controller ai_controller) {
            throw new NotImplementedException();
        }

        public Type execute(ai_controller ai_controller) {
            throw new NotImplementedException();
        }

        public Type exit(ai_controller ai_controller) {
            throw new NotImplementedException();
        }
    }


}
