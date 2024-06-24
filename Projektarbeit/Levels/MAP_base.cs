
namespace Projektarbeit.Levels {

    using System.Collections.Generic;
    using Core.util;
    using Core.world;
    using Core.defaults;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.controller;
    using Projektarbeit.characters.player.power_ups;

    internal class MAP_base : Map {

        private readonly Camera camera;
        private readonly Random random;
        private float timeStamp;
        private float timeInterval;
        private Dictionary<int, Action<Vector2>> enemyControllers;
        private Dictionary<int, Func<Vector2, PowerUp>> powerUps;   
        private const int MaxPowerUps = 5;
        private int powerUpCounter = 0;
        private const int PowerUpSpawnThreshold = 1;
        private bool bossFightTriggered = false;

        private int lastScore;


        public MAP_base()
        {
            use_garbage_collector = true;
            camera = Core.Game.Instance.camera;
            random = new Random();
            scoreGoal = 400;
            previousScoreGoal = 0;

            timeStamp = Game_Time.total;
            timeInterval = GetRandomTimeInterval();

            Set_Background_Image("assets/textures/background/background.png", 1.18f);

            Add_Player(Core.Game.Instance.player);

            InitializeEnemyControllers();
            InitializePowerUps();
        }

        public override void update(float deltaTime)
        {
            if (timeStamp + timeInterval <= Game_Time.total)
            {
                SpawnEnemies();
                if (powerUpCounter >= PowerUpSpawnThreshold)
                {
                    SpawnPowerUps();
                    powerUpCounter = 0;
                }
                else
                {
                    powerUpCounter++;
                }

                timeStamp = Game_Time.total;
                timeInterval = GetRandomTimeInterval();
            }

            if (Core.Game.Instance.Score != lastScore)
            {
                int scoreDifference = Core.Game.Instance.Score - lastScore;

                Game.Instance.GameState.AddXP(scoreDifference);

                lastScore = Core.Game.Instance.Score;
            }

            CheckScoreGoal();
        }

        private void CheckScoreGoal()
        {
            if (Core.Game.Instance.Score >= scoreGoal && !bossFightTriggered)
            {
                TriggerBossFight();
                bossFightTriggered = true;
            }
            else if (Core.Game.Instance.Score < scoreGoal)
            {
                bossFightTriggered = false;
            }
        }

        private void TriggerBossFight()
        {
            Console.WriteLine("Boss fight triggered!");
            CalculateScoreGoal();
        }

        public override void PlayerLevelUp()
        {
            Game.Instance.play_state = Core.Play_State.LevelUp;
        }

        private void CalculateScoreGoal()
        {
            previousScoreGoal = scoreGoal;
            scoreGoal += 200;
            if (scoreGoal - previousScoreGoal > 600)
            {
                scoreGoal = previousScoreGoal + 600;
            }
        }

        private void InitializeEnemyControllers()
        {
            enemyControllers = new Dictionary<int, Action<Vector2>>
            {
                { 0, spawnPosition => add_AI_Controller(new SwarmEnemyController(spawnPosition)) },
                { 1, spawnPosition => add_AI_Controller(new SniperEnemyController(spawnPosition)) },
                { 2, spawnPosition => add_AI_Controller(new SwarmEnemyController(spawnPosition)) },
                { 3, spawnPosition => add_AI_Controller(new TankEnemyController(spawnPosition)) },
            };
        }

        private void InitializePowerUps()
        {
            powerUps = new Dictionary<int, Func<Vector2, PowerUp>>();
            var unlockedPowerUps = Game.Instance.GameState.PowerUps.Where(p => !p.IsLocked).ToList();

            Console.WriteLine($"Unlocked power-ups: {unlockedPowerUps.Count}");

            for (int i = 0; i < unlockedPowerUps.Count; i++)
            {
                var powerUp = unlockedPowerUps[i];
                powerUps.Add(i, powerUpPosition => 
                {
                    // Corrected to only pass the parameters that match the constructor
                    var instance = Activator.CreateInstance(powerUp.GetType(), powerUpPosition);
                    if (instance == null)
                    {
                        throw new InvalidOperationException($"Failed to create an instance of {powerUp.GetType().Name}");
                    }
                    return (PowerUp)instance;
                });
            }
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
            if (!Game.Instance.GameState.PowerUps.Any(p => !p.IsLocked))
            {
                return;
            }

            double spawnRate = Core.Game.Instance.player.health < 50 ? 0.2 : 0.1;

            if (random.NextDouble() < spawnRate && allPowerUps.Count < MaxPowerUps)
            {
                Vector2 powerUpPosition = new(random.Next(-400, 400), random.Next(-400, 400));

                if (ShouldSpawnHealthBoost())
                {
                    var healthBoost = new HealthIncrease(powerUpPosition);
                    Add_Game_Object(healthBoost);
                    allPowerUps.Add(healthBoost);
                }
                else
                {
                    SpawnPowerUp(powerUpPosition);
                }
            }
        }

        private bool ShouldSpawnHealthBoost()
        {
            if (!Game.Instance.GameState.PowerUps.Any(p => p.GetType() == typeof(HealthIncrease) && !p.IsLocked))
            {
                return false;
            }

            double spawnRate = Core.Game.Instance.player.health < 50 ? 0.7 : 0.5;
            return random.NextDouble() < spawnRate;
        }

        private void SpawnPowerUp(Vector2 powerUpPosition)
        {
            int powerUpType = random.Next(0, powerUps.Count);
            if (powerUps.ContainsKey(powerUpType))
            {
                var powerUp = powerUps[powerUpType](powerUpPosition);
                powerUp.transform.position = powerUpPosition;
                Add_Game_Object((Game_Object)powerUp);
                allPowerUps.Add(powerUp);
            }
            else
            {
                Console.WriteLine($"Error: PowerUp type {powerUpType} does not exist.");
            }
        }
    }
}
