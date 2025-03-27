namespace Hell.enemy
{
    using System;
    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;

    public class Attack : I_state<AI_Controller>
    {
        public bool exit(AI_Controller aiController) => true;

        public Type execute(AI_Controller aiController, float delta_time)
        {
            Type nextState = typeof(Attack);

            foreach (Character character in aiController.characters)
            {
                if (character is CH_base_NPC npc)
                {
                    npc.Attack();
                    if (!npc.IsPlayerInAttackRange())
                    {
                        nextState = typeof(Pursue);
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
                    npc.set_animation_from_anim_data(npc.attackAnim);
                }
            }

            return true;
        }
    }
}
