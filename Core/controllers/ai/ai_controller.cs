namespace Core.Controllers.ai {
    using Core.world;
    using OpenTK.Mathematics;
    using System.Collections.Generic;

    public abstract class AI_Controller {

        private readonly Dictionary<string, I_AI_State> allStates = new ();
        private string currentState = string.Empty;

        // Add a list to hold the characters
        public List<Character> characters = new List<Character>();

        protected AI_Controller() {
        }

        public void Pre_Create_States(List<Type> states) {
            foreach(Type state_type in states)
                this.Create_State_Instance(state_type);
        }

        public void Set_Statup_State(Type state) {
            string className = state.Name;
            if(!this.allStates.ContainsKey(className))
                this.Create_State_Instance(state);

            this.currentState = this.Select_State_To_Execute(this.allStates[className].GetType());
            this.allStates[currentState].Enter(this);
        }

        internal void Update(float delta_time) {
            string newState = this.Select_State_To_Execute(this.allStates[this.currentState].Execute(this));
            if (this.currentState != newState) {
                this.allStates[this.currentState].Exit(this);
                this.allStates[newState].Enter(this);
            }
            this.currentState = newState;

            // Update all characters
            foreach (var character in characters) {
                character.Update(delta_time);
            }
        }

        public void force_set_state(Type state) {
            string newState = this.Select_State_To_Execute(state);
            if(this.currentState != newState) {
                this.allStates[this.currentState].Exit(this);
                this.allStates[newState].Enter(this);
            }
            this.currentState = newState;
        }

        // ------------------------------------------ private ------------------------------------------
        private string Select_State_To_Execute(Type state) {
            string className = state.Name;
            if(!this.allStates.ContainsKey(className))
                this.Create_State_Instance(state);

            return className;
        }

        private void Create_State_Instance(Type state) {
            if(typeof(I_AI_State).IsAssignableFrom(state)) {
                I_AI_State state_instance = (I_AI_State)Activator.CreateInstance(state);
                this.allStates.Add(state.Name, state_instance);
            }
            else 
                throw new InvalidOperationException($"Type [{state.Name}] does not implement [I_AI_state] interface.");
        }
    }
}