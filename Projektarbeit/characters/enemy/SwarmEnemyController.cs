namespace Hell.enemy {
    using Core.Controllers.ai;
    using Core.world;
    using Core;
    using OpenTK.Mathematics;
    using System;

    public class SwarmEnemyController : AIC_simple {
        public SwarmEnemyController(Vector2 origin) : base(false) {
            this.Origin = origin;
            Random random = new Random();
            float clusterRadius = 200f;

            int enemyCount = random.Next(8, 12);

            for (int i = 0; i < enemyCount; i++) {
                SwarmEnemy enemy = new SwarmEnemy();
                float angle = (float)random.NextDouble() * MathHelper.TwoPi;
                float radius = (float)random.NextDouble() * clusterRadius;
                Vector2 position = this.Origin + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Game.Instance.get_active_map().Add_empty_Character(enemy, position);
                this.characters.Add(enemy);
            }
            Set_Statup_State(typeof(Pursue));
        }

        public override bool Exit() {
            return base.Exit();
        }

        public override bool Enter() {
            return base.Enter();
        }

        public override Type Execute() {
            return base.Execute();
        }
    }
    public class EnterScreen : I_AI_State {
        public Type Execute(AI_Controller aiController) {
            foreach (var character in aiController.characters) {
                SwarmEnemy npc = (SwarmEnemy)character;
                npc.Move();
            }
            return typeof(EnterScreen);
        }

        public bool Exit(AI_Controller aiController) {
            return false;
        }

        public bool Enter(AI_Controller aiController) {
            foreach (var character in aiController.characters) {
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);
            }
            return false;
        }
    }
    public class Pursue : I_AI_State {

        public Type Execute(AI_Controller aiController) {
            foreach (var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Pursue();
            }
            return typeof(Pursue);
        }

        public bool Exit(AI_Controller aiController) {
            return false;
        }

        public bool Enter(AI_Controller aiController) {
            return false;
        }
    }

    public class Attack : I_AI_State {
        public Type Execute(AI_Controller aiController) {
            return null;
        }

        public bool Exit(AI_Controller aiController) {
            return false;
        }

        public bool Enter(AI_Controller aiController) {
            return false;
        }
    }

    public class Retreat : I_AI_State {
        private Vector2 direction;

        public Retreat(Vector2 direction) {
            this.direction = direction;
        }

        public Type Execute(AI_Controller aiController) {
            return null;
        }

        public bool Exit(AI_Controller aiController) {
            return false;
        }

        public bool Enter(AI_Controller aiController) {
            return false;
        }
    }
}