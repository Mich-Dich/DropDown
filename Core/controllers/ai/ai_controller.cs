using Core.controllers;
using Core.util;
using Core.world;

namespace Core.Controllers.ai
{
    public class AI_Controller : I_Controller
    {
        public List<Character> characters { get; set; } = new List<Character>();
        private readonly state_machine<AI_Controller> state_machine;

        public AI_Controller(List<Character> characters)
        {
            this.characters = characters;
            state_machine = new state_machine<AI_Controller>(this);
        }

        public virtual void Update(float deltaTime)
        {
            state_machine.Update(deltaTime);
            foreach (Character character in characters)
            {
                character.Update(deltaTime);
            }
        }

        public state_machine<AI_Controller> get_state_machine()
        {
            return state_machine;
        }
    }
}
