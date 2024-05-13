namespace Core.Controllers.ai
{
    using Core.world;

    public abstract class AI_Controller : I_Controller
    {
        private readonly Dictionary<string, I_AI_State> allStates = new ();
        private string currentState = string.Empty;

        protected AI_Controller(Character character)
        {
            this.character = character;
        }

        public Character character { get; set; }

        public void Pre_Create_States(List<Type> states)
        {
            foreach (Type state_type in states)
            {
                this.Create_State_Instance(state_type);
            }
        }

        public void Set_Statup_State(I_AI_State state)
        {
            string className = state.GetType().Name;
            Console.WriteLine($"setting startup state: {className}");

            if (!this.allStates.ContainsKey(className))
            {
                this.Create_State_Instance(state.GetType());
            }

            this.currentState = this.Select_State_To_Execute(this.allStates[className].GetType());
        }

        internal void Update()
        {
            string newState = this.Select_State_To_Execute(this.allStates[this.currentState].Execute(this));

            // call exit/enter method if switching state
            if (this.currentState != newState)
            {
                this.allStates[this.currentState].Exit(this);
                this.allStates[newState].Enter(this);
            }

            this.currentState = newState;
        }

        // ------------------------------------------ private ------------------------------------------
        private string Select_State_To_Execute(Type state)
        {
            string className = state.Name;
            Console.WriteLine(className);

            if (!this.allStates.ContainsKey(className))
            {
                this.Create_State_Instance(state);
            }

            return className;
        }

        private void Create_State_Instance(Type state)
        {
            if (typeof(I_AI_State).IsAssignableFrom(state))
            {
                I_AI_State state_instance = (I_AI_State)Activator.CreateInstance(state);
                this.allStates.Add(state.Name, state_instance);
            }
            else
            {
                throw new InvalidOperationException($"Type [{state.Name}] does not implement [I_AI_state] interface.");
            }
        }
    }
}
