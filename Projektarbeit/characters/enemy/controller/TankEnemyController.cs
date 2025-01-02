using System;
using System.Collections.Generic;
using Core;
using Core.Controllers.ai;
using Core.world;
using OpenTK.Mathematics;
using Projektarbeit.characters.enemy.character;
using Projektarbeit.characters.enemy.States;
using Projektarbeit.particles; // <-- For ShockwaveEffect

namespace Projektarbeit.characters.enemy.controller
{
    public class TankEnemyController : AI_Controller
    {
        private const float ClusterRadius = 200f;
        private readonly Random random = new();

        public TankEnemyController(Vector2 origin)
            : base(new List<Character>())
        {
            characters = CreateEnemies(origin);
            get_state_machine().Set_Statup_State(typeof(Pursue));
        }

        private List<Character> CreateEnemies(Vector2 origin)
        {
            var enemies = new List<Character>();
            int enemyCount = random.Next(1, 6);

            for (int i = 0; i < enemyCount; i++)
            {
                enemies.Add(CreateEnemy(origin));
            }

            return enemies;
        }

        private TankEnemy CreateEnemy(Vector2 origin)
        {
            var enemy = new TankEnemy
            {
                Controller = this,
            };

            Vector2 position = GenerateRandomPosition(origin);
            Game.Instance.get_active_map().Add_Character(enemy, position, 0, true);

            // death_callback is invoked when the enemy's health hits 0
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
            return origin + (new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius);
        }

        private void MarkEnemyAsDead(TankEnemy enemy)
        {
            // Mark the enemy as dead and remove from the map
            enemy.IsDead = true;
            enemy.health = 0;
            enemy.auto_heal_amout = 0;

            Game.Instance.get_active_map().Remove_Game_Object(enemy);
            Game.Instance.get_active_map().allCharacter.Remove(enemy);
            characters.Remove(enemy);

            // Increase score
            Game.Instance.Score++;

            // Trigger particle effect when enemy dies
            ShockwaveEffect.Trigger(
                Game.Instance.get_active_map().particleSystem,   
                enemy.transform.position,                        
                scale: 10.0f,                                    
                maxSpeed: 50.0f,                                 
                particleLifetime: 0.4f,                         
                maxParticles: 2000                               
            );
        }
    }
}
