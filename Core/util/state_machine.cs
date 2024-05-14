using Core.Controllers.ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.util {

    public class state_machine {

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
            if(this.currentState != newState) {
                this.allStates[this.currentState].Exit(this);
                this.allStates[newState].Enter(this);
            }
            this.currentState = newState;
        }

        // ------------------------------------------ private ------------------------------------------

        private readonly Dictionary<string, I_state> allStates = new ();
        private string currentState = string.Empty;

        private string Select_State_To_Execute(Type state) {

            string className = state.Name;
            if(!this.allStates.ContainsKey(className))
                this.Create_State_Instance(state);

            return className;
        }

        private void Create_State_Instance(Type state) {

            if(typeof(I_state).IsAssignableFrom(state)) {

                I_state state_instance = (I_state)Activator.CreateInstance(state);
                this.allStates.Add(state.Name, state_instance);
            }
            else
                throw new InvalidOperationException($"Type [{state.Name}] does not implement [I_AI_state] interface.");
        }

    }

    public interface I_state {

        Type Execute(object payload);
        bool Exit(object payload);
        bool Enter(object payload);
    }
}
