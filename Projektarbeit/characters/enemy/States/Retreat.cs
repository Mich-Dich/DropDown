namespace Projektarbeit.characters.enemy.States
{
    using System;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using Projektarbeit.characters.enemy.character;

    public class Retreat : I_state<AI_Controller>
    {
        public bool exit(AI_Controller aiController) => true;

        public Type execute(AI_Controller aiController, float delta_time)
        {
            Type nextState = typeof(Retreat);
            foreach (Character character in aiController.characters)
            {
                if (character is SwarmEnemy npc)
                {
                    npc.Retreat();
                    if (!npc.IsHealthLow())
                    {
                        nextState = typeof(Pursue);
                    }
                }
            }

            return nextState;
        }

        public bool enter(AI_Controller aiController)
        {
            foreach (Character character in aiController.characters)
            {
                if (character is CH_base_NPC npc)
                {
                    npc.set_animation_from_anim_data(npc.walkAnim);
                }
            }

            return true;
        }
    }
}
