namespace Hell.enemy
{
    using System;
    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;

    public class Pursue : I_state<AI_Controller>
    {
        public bool exit(AI_Controller aiController) => true;

        public Type execute(AI_Controller aiController, float delta_time)
        {
            Type nextState = typeof(Pursue);
            foreach (Character character in aiController.characters)
            {
                if (character is CH_base_NPC npc)
                {
                    npc.Pursue();
                    if (npc.IsPlayerInAttackRange())
                    {
                        nextState = typeof(Attack);
                    }

                    if (npc.IsHealthLow())
                    {
                        nextState = typeof(Retreat);
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
