namespace Hell.enemy
{
    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using System;

    public class Death : I_state<AI_Controller>
    {
        public bool exit(AI_Controller aiController) => true;

        public bool enter(AI_Controller aiController)
        {
            Console.WriteLine("DEATH");
            foreach (Character character in aiController.characters)
            {
                if (character is CH_base_NPC npc)
                {
                    npc.set_animation_from_anim_data(npc.idle_anim);
                }
            }
            return true;
        }

        public Type execute(AI_Controller aiController, float delta_time) => typeof(Death);
    }
}
