namespace Hell.enemy
{
    using System;
    using System.Collections.Generic;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using Hell.player;
    using OpenTK.Mathematics;
    /*public class RoamerController : AI_Controller<Roamer>
    {
        public readonly float maxDistanceOffset = 80f;

        public RoamerController(Roamer character) : base(new List<Roamer> { character })
        {
            Game.Instance.get_active_map().Add_Character(character, new Vector2(0, -200), 0, true);
            get_state_machine().Set_Statup_State(typeof(RRoam));
            character.Controller = this.AsControllerOf<CH_base_NPC>()
                                 ?? throw new ArgumentNullException(nameof(CH_base_NPC));
        }
    }

    // --- AI State Classes ---

    public class RApproach : I_state<AI_Controller<Roamer>>
    {
        private Random random = new Random();
        private float distanceOffset = 0f;

        public bool enter(AI_Controller<Roamer> aiController)
        {
            Roamer character = aiController.characters[0];
            character.set_animation_from_anim_data(character.walk_anim);
            distanceOffset = (float)random.NextDouble() * 1000 % aiController.AsControllerOf<RoamerController>()!.maxDistanceOffset;
            return true;
        }

        public Type execute(AI_Controller<Roamer> aiController, float delta_time)
        {
            Roamer character = aiController.characters[0];
            CH_player player = (CH_player)Game.Instance.player;

            if (character.PlayerDistance < character.attack_range - distanceOffset)
            {
                return typeof(RRoam);
            }

            character.Move(player.transform.position - character.transform.position);
            return typeof(RApproach);
        }

        public bool exit(AI_Controller<Roamer> aiController) { return true; }
    }

    public class RRoam : I_state<AI_Controller> {

        private Random random = new Random();
        private float distanceOffset = 0;
        private float attackDelay = 10;
        private float timeEntered = 0f;

        public bool enter(AI_Controller aiController) {
            timeEntered = Game_Time.total;

            RoamerController controller = (RoamerController)aiController;
            Roamer character = (Roamer)controller.character;
            character.set_animation_from_anim_data(character.walk_anim);
            distanceOffset = (float)random.NextDouble() * 1000 % controller.maxDistanceOffset;
            attackDelay = (float)random.NextDouble() * 2 + 1;

            return true;
        }

        public Type execute(AI_Controller aiController, float delta_time) {

            Roamer character = (Roamer)aiController.character;
            CH_player player = (CH_player)Game.Instance.player;
            if(character.IsDead) {
                return typeof(RDead);
            }
            if(character.PlayerDistance > character.attack_range + distanceOffset) {
                return typeof(RApproach);
            }
            if(character.PlayerDistance < character.attack_range + distanceOffset && Game_Time.total - timeEntered > attackDelay) {
                return typeof(RAttack);
            }
            //character.Roam();

            return typeof(RRoam);
        }

        public bool exit(AI_Controller aiController) { return true; }
    }

    public class RAttack : I_state<AI_Controller> {

        public bool enter(AI_Controller aiController) {

            RoamerController controller = (RoamerController)aiController;
            Roamer character = (Roamer)controller.character;
            character.set_animation_from_anim_data(character.attack_anim);

            return true;
        }

        public Type execute(AI_Controller aiController, float delta_time) {

            Roamer character = (Roamer)aiController.character;
            CH_player player = (CH_player)Game.Instance.player;

            character.Fire(player.transform.position - character.transform.position);

            return typeof(RRoam);
        }

        public bool exit(AI_Controller aiController) { return true; }
    }

    public class RDead : I_state<AI_Controller> {

        public bool enter(AI_Controller aiController) { return true; }

        public Type execute(AI_Controller aiController, float delta_time) {
            return typeof(RDead);
        }

        public bool exit(AI_Controller aiController) { return true; }
    }
    */
}