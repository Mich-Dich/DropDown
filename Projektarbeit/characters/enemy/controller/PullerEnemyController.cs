namespace Projektarbeit.characters.enemy.controller
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Controllers.ai;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.character;
    using Projektarbeit.characters.enemy.States;

    public class PullerEnemyController : AI_Controller
    {
        private const float ClusterRadius = 200f;
        private readonly Random random = new();

        public PullerEnemyController(Vector2 origin)
            : base(new List<Character>())
        {
            characters = CreateEnemies(origin);
            get_state_machine().Set_Statup_State(typeof(EnterScreen));
        }

        private List<Character> CreateEnemies(Vector2 origin)
        {
            var enemies = new List<Character>();
            int enemyCount = random.Next(1, 6);

            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = CreateEnemy(origin);
                enemies.Add(enemy);
            }

            return enemies;
        }

        private PullerEnemy CreateEnemy(Vector2 origin)
        {
            var enemy = new PullerEnemy { Controller = this };
            var position = GenerateRandomPosition(origin);
            Game.Instance.get_active_map().Add_Character(enemy, position, 0, true);

            enemy.death_callback = () =>
            {
                if (!enemy.IsDead)
                {
                    MarkEnemyAsDead(enemy);
                }
            };

            return enemy;
        }

        private Vector2 GenerateRandomPosition(Vector2 origin)
        {
            float angle = (float)random.NextDouble() * MathHelper.TwoPi;
            float radius = (float)random.NextDouble() * ClusterRadius;
            return origin + (new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius);
        }

        private void MarkEnemyAsDead(PullerEnemy enemy)
        {
            enemy.IsDead = true;
            enemy.health = 0;
            enemy.auto_heal_amout = 0;
            Game.Instance.get_active_map().Remove_Game_Object(enemy);
            Game.Instance.get_active_map().allCharacter.Remove(enemy);
            characters.Remove(enemy);
            Game.Instance.Score++;
            Console.WriteLine("PullerEnemyController: MarkEnemyAsDead");
            Console.WriteLine("Score: " + Game.Instance.Score);
        }
    }
}
