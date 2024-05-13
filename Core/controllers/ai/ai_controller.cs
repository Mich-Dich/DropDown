
namespace Core.controllers.ai {

using Core.world;

    public abstract class AI_Controller : I_Controller {

        public Character character { get; set; }

        protected AI_Controller(Character character) {

            this.character = character;
        }

        /// <summary>
        /// This function creates the state-mashines inital states. Should only be used when state creation ist costly. Cheap states can be created on the fly
        /// </summary>
        /// <param name="states">A list of Type objects representing classes that implement the [I_AI_state] interface.</param>
        /// <exception cref="InvalidOperationException">Thrown when a provided state type does not implement the [I_AI_state] interface.</exception>
        public void Pre_Create_States(List<Type> states) {

            foreach(Type state_type in states)
                Create_State_Instance(state_type);
        }

        /// <summary>
        /// Sets the initial state for the state machine. Expensive states should be pre-created using [pre_create_states()] and provided in the list to optimize performance.
        /// </summary>
        /// <param name="state">The initial state implementing the <see cref="I_AI_state"/> interface.</param>
        /// <exception cref="InvalidOperationException">Thrown when the provided state does not implement the <see cref="I_AI_state"/> interface.</exception>
        public void Set_Statup_State(I_AI_State state) {

            string className = state.GetType().Name;
            Console.WriteLine($"setting startup state: {className}"); // This will print the class name as a string

            if(!allStates.ContainsKey(className))       // create new states when they are needed
                Create_State_Instance(state.GetType());
                        
            currentState = Select_State_To_Execute(allStates[className].GetType());
        }

        /// <summary>
        /// Updates the state machine by executing the current state's logic and handling state transitions.
        /// </summary>
        internal void Update() {

            string newState = Select_State_To_Execute(allStates[currentState].Execute(this));

            // call exit/enter method if switching state
            if(currentState != newState) {

                allStates[currentState].Exit(this);
                allStates[newState].Enter(this);
            }
            currentState = newState;      // Use Updated state in next Update
        }

        // ------------------------------------------ private ------------------------------------------

        private string currentState = "";
        private Dictionary<string, I_AI_State> allStates = new Dictionary<string, I_AI_State>();

        /// <summary>
        /// Selects or creates an instance of a state based on the provided type.
        /// </summary>
        /// <param name="state">The Type of the state to select or create.</param>
        /// <returns>The name of the selected or created state.</returns>
        private string Select_State_To_Execute(Type state) {

            string className = state.Name;
            Console.WriteLine(className); // This will print the class name as a string

            if(!allStates.ContainsKey(className))       // create new states when they are needed
                Create_State_Instance(state);

            return className;
        }

        /// <summary>
        /// Creates an instance of a state if it implements the I_AI_state interface and adds it to the allStates dictionary.
        /// </summary>
        /// <param name="state">The Type of the state to create.</param>
        private void Create_State_Instance(Type state) {

            if(typeof(I_AI_State).IsAssignableFrom(state)) {    // Ensure the type implements I_AI_state

                I_AI_State state_instance = (I_AI_State)Activator.CreateInstance(state); // Create an instance of the state class
                allStates.Add(state.Name, state_instance);
            }
            else
                throw new InvalidOperationException($"Type [{state.Name}] does not implement [I_AI_state] interface.");
        }

    }
}
