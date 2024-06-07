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

    public class SniperEnemyController : AI_Controller
    {
        private const float ClusterRadius = 200f;
        private const int MinEnemyCount = 8;
        private const int MaxEnemyCount = 12;

        public SniperEnemyController(Vector2 origin)
            : base(new List<Character>())
        {
            characters = CreateEnemies(origin);
            get_state_machine().Set_Statup_State(typeof(EnterScreen));
        }

        private List<Character> CreateEnemies(Vector2 origin)
        {
            var enemies = new List<Character>();
            var random = new Random();
            int enemyCount = random.Next(MinEnemyCount, MaxEnemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                var enemy = CreateEnemy(origin, random);
                enemies.Add(enemy);
            }

            return enemies;
        }

        private SniperEnemy CreateEnemy(Vector2 origin, Random random)
        {
            var enemy = new SniperEnemy { Controller = this };

            Vector2 position = GetRandomPosition(origin, random);
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

        private Vector2 GetRandomPosition(Vector2 origin, Random random)
        {
            float angle = (float)random.NextDouble() * MathHelper.TwoPi;
            float radius = (float)random.NextDouble() * ClusterRadius;
            return origin + (new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius);
        }

        private void MarkEnemyAsDead(SniperEnemy enemy)
        {
            enemy.IsDead = true;
            enemy.health = 0;
            enemy.auto_heal_amout = 0;
            Game.Instance.get_active_map().Remove_Game_Object(enemy);
            Game.Instance.get_active_map().allCharacter.Remove(enemy);
            characters.Remove(enemy);
            Game.Instance.Score++;
        }
    }
}