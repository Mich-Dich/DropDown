using OpenTK.Mathematics;
using Core.Controllers.ai;
using Core.world;
using Projektarbeit.characters.enemy.character;
using Projektarbeit.characters.enemy.States;

namespace Projektarbeit.characters.enemy.controller
{
    public class BossController : AI_Controller
    {
        public BossController(Vector2 origin)
            : base(new List<Character>())
        {
            CH_base_NPC boss = new Boss(this);
            characters.Add(boss);
            Game.Instance.get_active_map().Add_Character(boss, origin, 0, true);
            get_state_machine().Set_Statup_State(typeof(Pursue));
        }
    }   
}