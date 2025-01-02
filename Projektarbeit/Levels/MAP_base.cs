namespace Projektarbeit.Levels {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.util;
    using Core.world;
    using Core.defaults;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.controller;
    using Projektarbeit.characters.player.power_ups;
    using Core.render;
    using Core.Particles;
    using Projektarbeit.characters.enemy.character;
    using Projektarbeit.particles;

    internal class MAP_base : Map {
        private readonly Camera camera;
        private readonly Random random = new Random();
        private float timeStamp;
        private float timeInterval;
        private Dictionary<int, Action<Vector2>> enemyControllers;
        private Dictionary<int, Func<Vector2, PowerUp>> powerUps;
        private const int MaxPowerUps = 5;
        private int powerUpCounter = 0;
        private const int PowerUpSpawnThreshold = 1;
        private bool bossFightTriggered = false;
        private int lastScore;

        // Timestamp for triggering shockwaves
        private float shockwaveTimeStamp = 0f;

        // Particle configuration
        private Func<Vector2> VelocityFunction;
        private Func<float> SizeFunction;
        private Func<float> RotationFunction;
        private ColorGradient ColorGradient;
        private Func<bool> IsAffectedByForcesFunction;

        public MAP_base() {
            use_garbage_collector = true;
            camera = Core.Game.Instance.camera;
            scoreGoal = 400;
            previousScoreGoal = 0;

            timeStamp = Game_Time.total;
            timeInterval = GetRandomTimeInterval();

            Set_Background_Image("assets/textures/background/background.png", 1.18f);
            Add_Player(Core.Game.Instance.player);

            //InitializeEnemyControllers();
            //all_game_objects.Add(new Spawner(new Vector2(0, -500), typeof(BossController), 1, true));
            InitializePowerUps();

            float scale = 10.0f;
            float maxSpeed = 50.0f; 
            float particleLifetime = 0.4f;

            // Original gradient
            ColorGradient = new ColorGradient();
            ColorGradient.AddColor(0.0f, new Vector4(0.0f, 0.8f, 1.0f, 1.0f));
            ColorGradient.AddColor(0.3f, new Vector4(0.0f, 0.6f, 1.0f, 0.7f));
            ColorGradient.AddColor(0.6f, new Vector4(0.0f, 0.4f, 1.0f, 0.4f));
            ColorGradient.AddColor(1.0f, new Vector4(0.0f, 0.0f, 0.5f, 0.0f));

            // Bubble size function
            Func<float, float> sizeOverLifeFunction = t => MathF.Pow(MathF.Sin(MathF.PI * t), 0.5f);

            VelocityFunction = () => {
                float angle = Random.Shared.NextSingle() * MathHelper.TwoPi;
                Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

                float speedVariation = (Random.Shared.NextSingle() * 0.5f) + 0.75f;
                float particleSpeed = maxSpeed * speedVariation;
                float adjustedScale = scale * 0.3f;
                return direction * particleSpeed * adjustedScale;
            };

            SizeFunction = () => 8.0f;

            RotationFunction = () => 0f;

            // No forces
            IsAffectedByForcesFunction = () => false;

            shockwaveTimeStamp = Game_Time.total - 5.0f;
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);

            if (timeStamp + timeInterval <= Game_Time.total) {
                //SpawnEnemies();
                if (powerUpCounter >= PowerUpSpawnThreshold) {
                    SpawnPowerUps();
                    powerUpCounter = 0;
                } else {
                    powerUpCounter++;
                }
                timeStamp = Game_Time.total;
                timeInterval = GetRandomTimeInterval();
            }

            if (Core.Game.Instance.Score != lastScore) {
                int scoreDifference = Core.Game.Instance.Score - lastScore;
                Game.Instance.GameState.AddXP(scoreDifference);
                lastScore = Core.Game.Instance.Score;
            }

            CheckScoreGoal();

            if (Game_Time.total - shockwaveTimeStamp >= 5.0f) {
                Console.WriteLine("Spawning XP particles!");
                
                XPParticleEffect.Create(
                    this.particleSystem,
                    amount: 10,
                    position: new Vector2(0, 0),
                    attractDistance: 200.0f,
                    collectDistance: 50.0f,
                    maxAttractForce: 450.0f,
                    maxSpeed: 400.0f,
                    damping: 0.95f
                );

                shockwaveTimeStamp = Game_Time.total;
            }
        }

        private void CheckScoreGoal() {
            if (Core.Game.Instance.Score >= scoreGoal && !bossFightTriggered) {
                TriggerBossFight();
                bossFightTriggered = true;
            } else if (Core.Game.Instance.Score < scoreGoal)
                bossFightTriggered = false;
        }

        private void TriggerBossFight() {
            Console.WriteLine("Boss fight triggered!");
            CalculateScoreGoal();
        }

        public override void PlayerLevelUp() {
            Game.Instance.play_state = Core.Play_State.LevelUp;
        }

        private void CalculateScoreGoal() {
            previousScoreGoal = scoreGoal;
            scoreGoal += 200;
            if (scoreGoal - previousScoreGoal > 600)
                scoreGoal = previousScoreGoal + 600;
        }

        private void InitializeEnemyControllers() {
            enemyControllers = new Dictionary<int, Action<Vector2>> {
                { 0, spawnPosition => add_AI_Controller(new SwarmEnemyController(spawnPosition)) },
                { 1, spawnPosition => add_AI_Controller(new TankEnemyController(spawnPosition)) },
                { 2, spawnPosition => add_AI_Controller(new ExplosivEnemyController(spawnPosition)) },
                { 3, spawnPosition => add_AI_Controller(new SniperEnemyController(spawnPosition)) },
            };
        }

        private void InitializePowerUps() {
            powerUps = new Dictionary<int, Func<Vector2, PowerUp>>();
            var unlockedPowerUps = Game.Instance.GameState.PowerUps.Where(p => !p.IsLocked).ToList();
            Console.WriteLine($"Unlocked power-ups: {unlockedPowerUps.Count}");
            for (int i = 0; i < unlockedPowerUps.Count; i++) {
                var powerUp = unlockedPowerUps[i];
                powerUps.Add(i, powerUpPosition => {
                    PowerUp instance = null;
                    var saveData = Game.Instance.GameState.PowerUpsSaveData.FirstOrDefault(p => p.PowerUpType == powerUp.GetType().Name);
                    if (saveData != null) {
                        if (powerUp.GetType() == typeof(SpeedBoost))
                            instance = new SpeedBoost(powerUpPosition, saveData.SpeedBoost, saveData.Duration);
                        else if (powerUp.GetType() == typeof(FireRateBoost))
                            instance = new FireRateBoost(powerUpPosition, saveData.FireDelayDecrease, saveData.Duration);
                        else if (powerUp.GetType() == typeof(HealthIncrease))
                            instance = new HealthIncrease(powerUpPosition);
                    }

                    if (instance == null)
                        throw new InvalidOperationException($"Failed to create an instance of {powerUp.GetType().Name}");

                    return instance;
                });
            }
        }

        private float GetRandomTimeInterval() {
            return random.Next(2, 4);
        }

        private void SpawnEnemies() {
            if (ShouldSpawnEnemies())
                SpawnEnemy();
        }

        private bool ShouldSpawnEnemies() {
            int maxEnemies = 10 + (Core.Game.Instance.Score / 10);
            int currentEnemies = Core.Game.Instance.get_active_map().allCharacter.Count;
            return currentEnemies < maxEnemies;
        }

        private void SpawnEnemy() {
            int score = Core.Game.Instance.Score;
            int enemyType = GetEnemyTypeBasedOnScore(score);
            Vector2 spawnPosition = new(random.Next(-250, 250), -600);
            enemyControllers[enemyType](spawnPosition);
        }

        private int GetEnemyTypeBasedOnScore(int score) {
            if (score <= 100)
                return random.Next(0, 2);
            else if (score <= 200)
                return random.Next(0, enemyControllers.Count);
            else
                return random.Next(1, enemyControllers.Count);
        }

        private void SpawnPowerUps() {
            if (!Game.Instance.GameState.PowerUps.Any(p => !p.IsLocked))
                return;

            double spawnRate = Core.Game.Instance.player.health < 50 ? 0.2 : 0.1;

            if (random.NextDouble() < spawnRate && allPowerUps.Count < MaxPowerUps) {
                Vector2 powerUpPosition = new(random.Next(-400, 400), random.Next(-400, 400));

                if (ShouldSpawnHealthBoost()) {
                    var healthBoost = new HealthIncrease(powerUpPosition);
                    Add_Game_Object(healthBoost);
                    allPowerUps.Add(healthBoost);
                } else {
                    SpawnPowerUp(powerUpPosition);
                }
            }
        }

        private bool ShouldSpawnHealthBoost() {
            if (!Game.Instance.GameState.PowerUps.Any(p => p.GetType() == typeof(HealthIncrease) && !p.IsLocked))
                return false;

            double spawnRate = Core.Game.Instance.player.health < 50 ? 0.7 : 0.5;
            return random.NextDouble() < spawnRate;
        }

        private void SpawnPowerUp(Vector2 powerUpPosition) {
            int powerUpType = random.Next(0, powerUps.Count);
            if (powerUps.ContainsKey(powerUpType)) {
                var powerUp = powerUps[powerUpType](powerUpPosition);
                powerUp.transform.position = powerUpPosition;
                Add_Game_Object((Game_Object)powerUp);
                allPowerUps.Add(powerUp);
            } else
                Console.WriteLine($"Error: PowerUp type {powerUpType} does not exist.");
        }
    }
}
