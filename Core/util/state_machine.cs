namespace Core.util {

    public interface I_state<T> {

        Type execute(T aiController, float delta_time);
        bool exit(T aiController);
        bool enter(T aiController);
    }


    public sealed class state_machine<T> {

        public object user_pointer { get; set; }

        public state_machine(T payload) { this.payload = payload; }

        // ------------------------------------------ public ------------------------------------------

        public void Pre_Create_States(List<Type> states) {

            foreach(Type state_type in states)
                Create_State_Instance(state_type);
        }

        public void Set_Statup_State(Type state) {

            string className = state.Name;
            if(!allStates.ContainsKey(className))
                Create_State_Instance(state);

            currentState = Select_State_To_Execute(allStates[className].GetType());
            allStates[currentState].enter(payload);
        }

        public void Update(float delta_time) { change_state(allStates[currentState].execute(payload, delta_time)); }

        public void force_set_state(Type state) { change_state(state); }

        // ------------------------------------------ private ------------------------------------------

        public readonly Dictionary<string, I_state<T>> allStates = new ();
        public string currentState = string.Empty;
        private readonly T payload;

        private void change_state(Type state) {

            string newState = this.Select_State_To_Execute(state);
            if(this.currentState != newState) {
                this.allStates[this.currentState].exit(payload);
                this.allStates[newState].enter(payload);
            }
            this.currentState = newState;
        }

        private string Select_State_To_Execute(Type state) {

            string className = state.Name;
            if(!allStates.ContainsKey(className))
                Create_State_Instance(state);

            return className;
        }

        private void Create_State_Instance(Type state) {

            if(typeof(I_state<T>).IsAssignableFrom(state)) {

                I_state<T> state_instance = (I_state<T>)Activator.CreateInstance(state);
                allStates.Add(state.Name, state_instance);
            }
            else
                throw new InvalidOperationException($"Type [{state.Name}] does not implement [I_state] interface.");
        }

    }

}
