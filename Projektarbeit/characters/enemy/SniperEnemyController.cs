namespace Hell.enemy
{
    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using System;
    using System.Collections.Generic;

    public class SniperEnemyController : AI_Controller
    {
        public SniperEnemyController(Vector2 origin) : base(new List<Character>())
        {
            this.characters = CreateEnemies(origin);
            get_state_machine().Set_Statup_State(typeof(EnterScreen));
        }

        private List<Character> CreateEnemies(Vector2 origin)
        {
            var enemies = new List<Character>();
            Random random = new Random();
            float clusterRadius = 200f;
            int enemyCount = random.Next(8, 12); 

            for (int i = 0; i < enemyCount; i++)
            {
                SniperEnemy enemy = new SniperEnemy();
                enemy.Controller = this;

                float angle = (float)random.NextDouble() * MathHelper.TwoPi;
                float radius = (float)random.NextDouble() * clusterRadius;
                Vector2 position = origin + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Game.Instance.get_active_map().Add_Character(enemy, position, 0, true);
                enemies.Add(enemy);

                enemy.death_callback = () =>
                {
                    if (!enemy.IsDead)
                    {
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
            return enemies;
        }
    }
}
