using OpenTK.Mathematics;
using Core.Controllers.ai;
using Core.world;
using Projektarbeit.characters.enemy.character;
using Projektarbeit.characters.enemy.States;
using Projektarbeit.particles;

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
            
            // Add death callback for wave progress tracking
            boss.death_callback = () =>
            {
                // XP drop (super orbs for bosses)
                XPParticleEffect.CreateByType(
                    Core.Game.Instance.get_active_map().particleSystem,
                    5,
                    boss.transform.position,
                    XPOrbType.Super,
                    attractDistance: 300.0f,
                    collectDistance: 50.0f,
                    maxAttractForce: 500.0f,
                    maxSpeed: 450.0f,
                    damping: 0.95f
                );
                
                var currentWave = Projektarbeit.Levels.Wave.GetCurrentWave();
                currentWave?.EnemyDefeated();
            };
            
            get_state_machine().Set_Statup_State(typeof(Pursue));
        }
    }   
}