
using Core.game_objects;

namespace Core.controllers.ai {

    public abstract class ai_controller : I_controller {

        public character character { get; set; }

        /// <summary>
        /// This function creates the state-mashines inital states. Should be used when state creation ist costly. Cheap states can be created on the fly
        /// </summary>
        /// <param name="states">A list of Type objects representing classes that implement the [I_AI_state] interface.</param>
        /// <exception cref="InvalidOperationException">Thrown when a provided state type does not implement the [I_AI_state] interface.</exception>
        public void register_state(List<Type> states) {

            foreach(Type state_type in states)
                create_state_instance(state_type);
        }

        public void update() {

            string new_state = select_state_to_execute(_all_states[current_state].execute(this));

            // call exit/enter method if switching state
            if(current_state != new_state) {

                _all_states[current_state].exit(this);
                _all_states[new_state].enter(this);
            }
            current_state = new_state;      // use updated state in next update
        }

        // ------------------------------------------ private ------------------------------------------

        private string current_state;
        private Dictionary<string, I_AI_state> _all_states = new Dictionary<string, I_AI_state>();


        /// <summary>
        /// Selects or creates an instance of a state based on the provided type.
        /// </summary>
        /// <param name="state">The Type of the state to select or create.</param>
        /// <returns>The name of the selected or created state.</returns>
        private string select_state_to_execute(Type state) {

            string className = state.GetType().Name;
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
