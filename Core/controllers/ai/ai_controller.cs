namespace Core.controllers.ai
{

    public abstract class ai_controller {

        public void register_state(List<Type> states) {

            foreach(Type state_type in states) {

                if(typeof(I_AI_state).IsAssignableFrom(state_type)) {    // Ensure the type implements I_AI_state

                    I_AI_state state_instance = (I_AI_state)Activator.CreateInstance(state_type); // Create an instance of the state class
                    _all_states.Add(state_type.Name, state_instance);
                }
                else
                    throw new Exception($"Type [{state_type.Name}] does not implement [I_AI_state] interface.");
            }

        }

        public void update() {

            if(_all_states.ContainsKey(current_state))
                current_state = convert_state_to_string(_all_states[current_state].execute_state(this));

            else
                throw new Exception($"AI state-maschine tried to acces unknown state: [{current_state}]");
        }

        // ------------------------------------------ private ------------------------------------------

        private string current_state;
        private Dictionary<string, I_AI_state> _all_states = new Dictionary<string, I_AI_state>();

        private string convert_state_to_string(I_AI_state state) {

            string className = state.GetType().Name;
            Console.WriteLine(className); // This will print the class name as a string
            return className;
        }

    }
}
