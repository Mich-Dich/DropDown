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
                Game.Instance.get_active_map().Add_Character(enemy, position);
                this.characters.Add(enemy);
                enemy.death_callback = () =>
                {
                    if (!enemy.IsDead) {
                        enemy.IsDead = true;
                        enemy.health = 0;
                        enemy.auto_heal_amout = 0;
                        Game.Instance.get_active_map().Remove_Game_Object(enemy);
                        Game.Instance.get_active_map().allCharacter.Remove(enemy);
                        this.characters.Remove(enemy);
                        Game.Instance.Score++;
                    }
                };
            }

            get_state_machine().Set_Statup_State(typeof(EnterScreen));
        }

    }

    public class EnterScreen : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aiController) { return true; }
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            Type nextState = typeof(EnterScreen);
            foreach(var character in aiController.characters) {
                SwarmEnemy npc = (SwarmEnemy)character;
                npc.Move();
                if(npc.IsPlayerInRange())
                    nextState = typeof(Pursue);
            }

            return nextState;
        }

        public bool enter(AI_Swarm_Controller aiController) {

            foreach(var character in aiController.characters)
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);
            
            return true;
        }

    }

    public class Pursue : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aiController) { return true; }
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            Type nextState = typeof(Pursue);
            foreach(var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Pursue();
                if(enemy.IsPlayerInAttackRange()) 
                    nextState = typeof(Attack);
                if(enemy.IsHealthLow()) 
                    nextState = typeof(Retreat);
            }

            return nextState;
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

            Type nextState = typeof(Attack);
            foreach(var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Attack();
                if(!enemy.IsPlayerInAttackRange()) 
                    nextState = typeof(Pursue);
                if(enemy.IsHealthLow()) 
                    nextState = typeof(Retreat);
            }

            return nextState;
        }

        public bool enter(AI_Swarm_Controller aiController) {

            foreach(var character in aiController.characters) 
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).attack_anim);
            
            return true;
        }
    }

    public class Retreat : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aiController) { return true; }
        public Type execute(AI_Swarm_Controller aiController, float delta_time) {

            Type nextState = typeof(Retreat);
            foreach(var character in aiController.characters) {
                SwarmEnemy enemy = (SwarmEnemy)character;
                enemy.Retreat();
                if(!enemy.IsHealthLow())
                    nextState = typeof(Pursue);
            }

            return nextState;
        }

        public bool enter(AI_Swarm_Controller aiController) {

            foreach(var character in aiController.characters)
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);

            return true;
        }
    }

    public class Death : I_state<AI_Swarm_Controller> {

        public bool exit(AI_Swarm_Controller aI_Controller) { return true; }
        public bool enter(AI_Swarm_Controller aI_Controller) {

            Console.WriteLine($"DEATH");
            foreach(var character in aI_Controller.characters) 
                ((CH_base_NPC)character).set_animation_from_anim_data(((CH_base_NPC)character).idle_anim);
            
            return true;
        }

        public Type execute(AI_Swarm_Controller aI_Controller, float delta_time) { return typeof(Death); }
    }
}
