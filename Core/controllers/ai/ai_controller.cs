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
            if(Game.Instance.play_state == Play_State.LevelUp) { return; }
            if(Game.Instance.play_state == Play_State.InGameMenu) { return; }
            if(Game.Instance.play_state == Play_State.PauseMenuSkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PauseAbilitySkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PausePowerupSkillTree) { return; }


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
