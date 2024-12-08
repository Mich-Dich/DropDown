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

        // Particle constants: Increase speed for a more "shockwave" effect
        private const float PARTICLE_BASE_SPEED = 40.0f;
        private const float PARTICLE_SIZE_START = 5.0f;
        private const float PARTICLE_SIZE_END = 1.0f;
        private const float ROTATION_SPEED = 45.0f; // degrees per second

        public MAP_base() {
            use_garbage_collector = true;
            camera = Core.Game.Instance.camera;
            scoreGoal = 400;
            previousScoreGoal = 0;

            timeStamp = Game_Time.total;
            timeInterval = GetRandomTimeInterval();

            Set_Background_Image("assets/textures/background/background.png", 1.18f);
            Add_Player(Core.Game.Instance.player);

            InitializeEnemyControllers();
            InitializePowerUps();

            // Setup Particle Functions and ColorGradient
            ColorGradient = new ColorGradient();
            ColorGradient.AddColor(0.0f, new Vector4(0.0f, 0.8f, 1.0f, 1.0f)); // Bright cyan
            ColorGradient.AddColor(0.3f, new Vector4(0.0f, 0.6f, 1.0f, 0.7f)); // Medium blue
            ColorGradient.AddColor(0.6f, new Vector4(0.0f, 0.4f, 1.0f, 0.4f)); // Darker blue
            ColorGradient.AddColor(1.0f, new Vector4(0.0f, 0.0f, 0.5f, 0.0f)); // Dark blue, fade out

            // Velocity: Radial outward direction with slight random variation
            VelocityFunction = () => {
                float angle = Random.Shared.NextSingle() * MathHelper.TwoPi;
                float speedFactor = 0.8f + 0.4f * Random.Shared.NextSingle(); // vary speed slightly
                return new Vector2(
                    MathF.Cos(angle) * PARTICLE_BASE_SPEED * speedFactor,
                    MathF.Sin(angle) * PARTICLE_BASE_SPEED * speedFactor
                );
            };

            SizeFunction = () => MathHelper.Lerp(PARTICLE_SIZE_START, PARTICLE_SIZE_END, Random.Shared.NextSingle());

            RotationFunction = () => ROTATION_SPEED * (Random.Shared.NextSingle() * 2 - 1);

            // No forces, so they just fly outward
            IsAffectedByForcesFunction = () => false;
        }

        public override void update(float deltaTime) {
            base.update(deltaTime); // Ensure the base update logic is called

            // Spawn logic for enemies and power-ups
            if (timeStamp + timeInterval <= Game_Time.total) {
                SpawnEnemies();
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

            // Every second, create a shockwave
            if (Game_Time.total - shockwaveTimeStamp >= 1.0f) {
                Console.WriteLine("Creating a shockwave burst!");

                // Emit a burst of particles all at once (non-continuous)
                this.particleSystem.AddEmitter(new Emitter(
                    position: new Vector2(0,0),
                    emissionRate: 600,        // emit 200 particles almost instantly
                    continuous: false,        // one-time burst
                    particleLifetime: 3.0f,   // particles live for 3 seconds
                    VelocityFunction,
                    SizeFunction,
                    RotationFunction,
                    ColorGradient,
                    IsAffectedByForcesFunction,
                    maxParticles: 2000
                ));

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
