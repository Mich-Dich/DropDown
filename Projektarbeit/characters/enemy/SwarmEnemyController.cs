namespace Hell.enemy {

    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using System;

    public class SwarmEnemyController : AI_Swarm_Controller {

        public Vector2 Origin = new Vector2();

        public SwarmEnemyController(Vector2 origin) : base() {

            this.Origin = origin;
            Random random = new Random();
            float clusterRadius = 200f;
            int enemyCount = random.Next(8, 12);

            for(int i = 0; i < enemyCount; i++) {
                SwarmEnemy enemy = new SwarmEnemy { Controller = this };
                float angle = (float)random.NextDouble() * MathHelper.TwoPi;
                float radius = (float)random.NextDouble() * clusterRadius;
                Vector2 position = this.Origin + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Game.Instance.get_active_map().Add_empty_Character(enemy, position);
                this.characters.Add(enemy);
            }

            get_state_machine().Set_Statup_State(typeof(EnterScreen));
        }

    }

    public class EnterScreen : I_state<AI_Swarm_Controller> {
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            foreach(var character in aiController.characters) {
                SwarmEnemy npc = (SwarmEnemy)character;
                npc.Move();
                if(npc.IsPlayerInRange())
                    return typeof(Pursue);
            }

            aiController.characters.RemoveAll(character => (character as SwarmEnemy).IsDead);

            return typeof(EnterScreen);
        }

        public bool exit(AI_Swarm_Controller aiController) {
            return true;
        }

        public bool enter(AI_Swarm_Controller aiController) {
            foreach(var character in aiController.characters) {
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);
            }
            return true;
        }

    }

    public class Pursue : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aiController) { return true; }
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            foreach(var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Pursue();
                if(enemy.IsPlayerInAttackRange()) {
                    return typeof(Attack);
                }
                if(enemy.IsHealthLow()) {
                    return typeof(Retreat);
                }
            }

            aiController.characters.RemoveAll(character => (character as SwarmEnemy).IsDead);

            return typeof(Pursue);
        }

        public bool enter(AI_Swarm_Controller aiController) {
            foreach(var character in aiController.characters)
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).walk_anim);
            return true;
        }
    }

    public class Attack : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aiController) { return true; }
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            foreach(var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Attack();
                if(!enemy.IsPlayerInAttackRange()) 
                    return typeof(Pursue);
                if(enemy.IsHealthLow()) 
                    return typeof(Retreat);
            }

            aiController.characters.RemoveAll(character => (character as SwarmEnemy).IsDead);

            return typeof(Attack);
        }

        public bool enter(AI_Swarm_Controller aiController) {

            foreach(var character in aiController.characters) {
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).attack_anim);
            }
            return true;
        }
    }

    public class Retreat : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aiController) { return true; }
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            foreach(var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Retreat();
                if(!enemy.IsHealthLow())
                    return typeof(Pursue);
            }

            aiController.characters.RemoveAll(character => (character as SwarmEnemy).IsDead);

            return typeof(Retreat);
        }

        public bool enter(AI_Swarm_Controller aiController) {

            foreach(var character in aiController.characters)
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);
            return true;
        }
    }
}