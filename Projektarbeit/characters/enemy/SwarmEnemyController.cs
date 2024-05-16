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
            }

            Vector2 direction = new Vector2(1, 0);
            float swarmPatternFactor = 1.0f;
            float randomnessFactor = 0.5f;
            Set_Statup_State(new EnterScreen(direction, swarmPatternFactor, randomnessFactor));
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
        private Vector2 direction;
        private float swarmPatternFactor;
        private float randomnessFactor;

        public EnterScreen(Vector2 direction, float swarmPatternFactor, float randomnessFactor) {
            this.direction = direction;
            this.swarmPatternFactor = swarmPatternFactor;
            this.randomnessFactor = randomnessFactor;
        }

        public Type Execute(AI_Controller aiController) {
            foreach (var character in aiController.characters) {
                CH_base_NPC npc = (CH_base_NPC)character;
                Vector2 directionToTarget = direction - npc.transform.position;
                directionToTarget.Normalize();
                directionToTarget *= npc.movement_speed;
                Box2DX.Common.Vec2 directionToTargetBox2D = new Box2DX.Common.Vec2(directionToTarget.X, directionToTarget.Y);
                npc.Add_Linear_Velocity(directionToTargetBox2D);
            }
            return null;
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
        private Game_Object player;

        public Pursue(Game_Object player) {
            this.player = player;
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