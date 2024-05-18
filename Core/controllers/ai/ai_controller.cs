
namespace Core.Controllers.ai {

    using Core.controllers;
    using Core.util;
    using Core.world;

    public abstract class AI_Controller : I_Controller {

        public Character character { get; set; } = new Character();

        public AI_Controller(Character character) {

            this.character = character;
            state_machine = new state_machine<AI_Controller>(this);
        }

        public virtual void Update(float delta_time) {

            state_machine.Update(delta_time);
            character.Update(delta_time);
        }

        public state_machine<AI_Controller> get_state_machine() { return state_machine; }

        // ------------------------------------------ private ------------------------------------------
        private state_machine<AI_Controller> state_machine;

    }


    public abstract class AI_Swarm_Controller : I_Controller {

        public List<Character> characters { get; set; } = new List<Character>();

        public AI_Swarm_Controller(List<Character>? characters = null) { 
            
            if(characters != null)
                this.characters = characters;
            state_machine = new state_machine<AI_Swarm_Controller>(this);
        }

        public virtual void Update(float delta_time) {

            state_machine.Update(delta_time);
            foreach (Character character in characters)
                character.Update(delta_time);
        }

        public state_machine<AI_Swarm_Controller> get_state_machine() { return state_machine; }

        //public void Pre_Create_States(List<Type> states) { state_machine.Pre_Create_States(states); }
        //public void Set_Statup_State(Type state) { state_machine.Set_Statup_State(state); }
        //public void force_set_state(Type state) { state_machine.force_set_state(state); }

        // ------------------------------------------ private ------------------------------------------
        private state_machine<AI_Swarm_Controller> state_machine;

    }

}
