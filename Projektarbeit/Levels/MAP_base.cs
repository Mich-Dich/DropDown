
namespace Projektarbeit.Levels {

    using System.Collections.Generic;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.controller;
    using Projektarbeit.characters.player.power_ups;

    internal class MAP_base : Map {

        private readonly Camera camera;
        private readonly Random random;
        private float timeStamp;
        private float timeInterval;
        private Dictionary<int, Action<Vector2>> enemyControllers;
        private Dictionary<int, Action<Vector2>> powerUps;

        public MAP_base() {

            use_garbage_collector = true;
            camera = Core.Game.Instance.camera;
            random = new Random();

            timeStamp = Game_Time.total;
            timeInterval = GetRandomTimeInterval();

            Set_Background_Image("assets/textures/background/background.png", 1.18f);

            Add_Player(Core.Game.Instance.player);

            InitializeEnemyControllers();
            InitializePowerUps();
        }

        public override void update(float deltaTime) {

            if (timeStamp + timeInterval <= Game_Time.total) {

                SpawnEnemies();
                SpawnPowerUps();
                timeStamp = Game_Time.total;
                timeInterval = GetRandomTimeInterval();
            }
        }

        private void InitializeEnemyControllers() {

            enemyControllers = new Dictionary<int, Action<Vector2>> {

                { 0, spawnPosition => add_AI_Controller(new SwarmEnemyController(spawnPosition)) },
                { 1, spawnPosition => add_AI_Controller(new SniperEnemyController(spawnPosition)) },
                { 2, spawnPosition => add_AI_Controller(new SwarmEnemyController(spawnPosition)) },
                { 3, spawnPosition => add_AI_Controller(new TankEnemyController(spawnPosition)) },
            };
        }

        private void InitializePowerUps() {

            powerUps = new Dictionary<int, Action<Vector2>> {

                { 0, powerUpPosition => Add_Game_Object(new SpeedBoost(powerUpPosition)) },
                { 1, powerUpPosition => Add_Game_Object(new HealthIncrease(powerUpPosition)) },
                { 2, powerUpPosition => Add_Game_Object(new FireRateBoost(powerUpPosition)) },
            };
        }

        private float GetRandomTimeInterval()
        {
            return random.Next(2, 4);
        }

        private void SpawnEnemies()
        {
            if (ShouldSpawnEnemies())
            {
                SpawnEnemy();
            }
        }

        private bool ShouldSpawnEnemies()
        {
            int maxEnemies = 10 + (Core.Game.Instance.Score / 10);
            int currentEnemies = Core.Game.Instance.get_active_map().allCharacter.Count;

            return currentEnemies < maxEnemies;
        }

        private void SpawnEnemy()
        {
            int enemyType = random.Next(0, enemyControllers.Count);
            Vector2 spawnPosition = new(random.Next(-250, 250), -600);

            enemyControllers[enemyType](spawnPosition);
        }

        private void SpawnPowerUps()
        {
            if (random.NextDouble() < 0.1)
            {
                Vector2 powerUpPosition = new(random.Next(-400, 400), random.Next(-400, 400));

                if (ShouldSpawnHealthBoost())
                {
                    Add_Game_Object(new HealthBoost(powerUpPosition));
                }
                else
                {
                    SpawnPowerUp(powerUpPosition);
                }
            }
        }

        private bool ShouldSpawnHealthBoost()
        {
            return Core.Game.Instance.player.health < 50 && random.NextDouble() < 0.5;
        }

        private void SpawnPowerUp(Vector2 powerUpPosition)
        {
            int powerUpType = random.Next(0, powerUps.Count);
            powerUps[powerUpType](powerUpPosition);
        }
    }
}
