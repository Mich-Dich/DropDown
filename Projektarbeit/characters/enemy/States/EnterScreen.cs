namespace Hell.enemy
{
    using System;
    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;

    public class EnterScreen : I_state<AI_Controller>
    {
        public bool exit(AI_Controller aiController) => true;

        public Type execute(AI_Controller aiController, float delta_time)
        {
            Type nextState = typeof(EnterScreen);
            foreach (Character character in aiController.characters)
            {
                if (character is CH_base_NPC npc)
                {
                    npc.Move();
                    if (npc.IsPlayerInRange())
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
                    npc.set_animation_from_anim_data(npc.idleAnim);
                }
            }

            return true;
        }
    }
}
