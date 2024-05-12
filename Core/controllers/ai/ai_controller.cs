﻿
namespace Core.controllers.ai {

using Core.world;

    public abstract class ai_controller : I_controller {

        public Character character { get; set; }

        protected ai_controller(Character character) {

            this.character = character;
        }

        /// <summary>
        /// This function creates the state-mashines inital states. Should only be used when state creation ist costly. Cheap states can be created on the fly
        /// </summary>
        /// <param name="states">A list of Type objects representing classes that implement the [I_AI_state] interface.</param>
        /// <exception cref="InvalidOperationException">Thrown when a provided state type does not implement the [I_AI_state] interface.</exception>
        public void pre_create_states(List<Type> states) {

            foreach(Type state_type in states)
                create_state_instance(state_type);
        }

        /// <summary>
        /// Sets the initial state for the state machine. Expensive states should be pre-created using [pre_create_states()] and provided in the list to optimize performance.
        /// </summary>
        /// <param name="state">The initial state implementing the <see cref="I_AI_state"/> interface.</param>
        /// <exception cref="InvalidOperationException">Thrown when the provided state does not implement the <see cref="I_AI_state"/> interface.</exception>
        public void set_statup_state(I_AI_state state) {

            string className = state.GetType().Name;
            Console.WriteLine($"setting startup state: {className}"); // This will print the class name as a string

            if(!_all_states.ContainsKey(className))       // create new states when they are needed
                create_state_instance(state.GetType());
                        
            current_state = select_state_to_execute(_all_states[className].GetType());
        }

        /// <summary>
        /// Updates the state machine by executing the current state's logic and handling state transitions.
        /// </summary>
        internal void update() {

            string new_state = select_state_to_execute(_all_states[current_state].execute(this));

            // call exit/enter method if switching state
            if(current_state != new_state) {

                _all_states[current_state].exit(this);
                _all_states[new_state].enter(this);
            }
            current_state = new_state;      // Use updated state in next update
        }

        // ------------------------------------------ private ------------------------------------------

        private string current_state = "";
        private Dictionary<string, I_AI_state> _all_states = new Dictionary<string, I_AI_state>();

        /// <summary>
        /// Selects or creates an instance of a state based on the provided type.
        /// </summary>
        /// <param name="state">The Type of the state to select or create.</param>
        /// <returns>The name of the selected or created state.</returns>
        private string select_state_to_execute(Type state) {

            string className = state.Name;
            Console.WriteLine(className); // This will print the class name as a string

            if(!_all_states.ContainsKey(className))       // create new states when they are needed
                create_state_instance(state);

            return className;
        }

        /// <summary>
        /// Creates an instance of a state if it implements the I_AI_state interface and adds it to the _all_states dictionary.
        /// </summary>
        /// <param name="state">The Type of the state to create.</param>
        private void create_state_instance(Type state) {

            if(typeof(I_AI_state).IsAssignableFrom(state)) {    // Ensure the type implements I_AI_state

                I_AI_state state_instance = (I_AI_state)Activator.CreateInstance(state); // Create an instance of the state class
                _all_states.Add(state.Name, state_instance);
            }
            else
                throw new InvalidOperationException($"Type [{state.Name}] does not implement [I_AI_state] interface.");
        }

    }
}
