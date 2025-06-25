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
        
        // Cache sound for level up
        private static readonly Sound levelUpSound = Resource_Manager.Get_Sound("assets/sounds/sample1.WAV");
        private float timeStamp;
        private float timeInterval;
        private Dictionary<int, Action<Vector2>> enemyControllers;
        private Dictionary<int, Func<Vector2, PowerUp>> powerUps;
        private const int MaxPowerUps = 5;
        private int powerUpCounter = 0;
        private const int PowerUpSpawnThreshold = 1;
        private bool bossFightTriggered = false;
        private int lastScore;

        // Improved powerup spawning system
        private float lastHealthPowerupSpawnTime = 0f;
        private float lastSpeedPowerupSpawnTime = 0f;
        private float lastFireRatePowerupSpawnTime = 0f;
        private const float HealthPowerupCooldown = 8f;  // 8 seconds between health powerups (was 15f)
        private const float SpeedPowerupCooldown = 12f;  // 12 seconds between speed powerups (was 20f)
        private const float FireRatePowerupCooldown = 15f; // 15 seconds between fire rate powerups (was 25f)
        private int consecutiveHealthPowerups = 0;
        private const int MaxConsecutiveHealthPowerups = 3; // Increased from 2 to 3

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
            scoreGoal = 150;
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
            // Play level up sound
            _ = levelUpSound.Play();
            
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
            
            // TEMPORARY: Ensure powerups are unlocked for testing
            EnsurePowerUpsUnlocked();
            
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

                    if (instance == null) {
                        // Fallback to default values if no save data
                        if (powerUp.GetType() == typeof(SpeedBoost))
                            instance = new SpeedBoost(powerUpPosition, 300f, 3f);
                        else if (powerUp.GetType() == typeof(FireRateBoost))
                            instance = new FireRateBoost(powerUpPosition, 0.1f, 4f);
                        else if (powerUp.GetType() == typeof(HealthIncrease))
                            instance = new HealthIncrease(powerUpPosition);
                    }

                    if (instance == null)
                        throw new InvalidOperationException($"Failed to create an instance of {powerUp.GetType().Name}");

                    return instance;
                });
            }
        }

        private void EnsurePowerUpsUnlocked() {
            // Ensure all powerup types exist and are unlocked for testing
            var existingPowerUps = Game.Instance.GameState.PowerUps;
            
            // Check for SpeedBoost
            if (!existingPowerUps.Any(p => p.GetType() == typeof(SpeedBoost))) {
                var speedBoost = new SpeedBoost(new Vector2(999, 999), 300f, 3f);
                speedBoost.IsLocked = false;
                speedBoost.Level = 1;
                Game.Instance.GameState.PowerUps.Add(speedBoost);
                Console.WriteLine("[PowerUp] Added and unlocked SpeedBoost for testing");
            }
            
            // Check for FireRateBoost
            if (!existingPowerUps.Any(p => p.GetType() == typeof(FireRateBoost))) {
                var fireRateBoost = new FireRateBoost(new Vector2(999, 999), 0.1f, 4f);
                fireRateBoost.IsLocked = false;
                fireRateBoost.Level = 1;
                Game.Instance.GameState.PowerUps.Add(fireRateBoost);
                Console.WriteLine("[PowerUp] Added and unlocked FireRateBoost for testing");
            }
            
            // Check for HealthIncrease
            if (!existingPowerUps.Any(p => p.GetType() == typeof(HealthIncrease))) {
                var healthIncrease = new HealthIncrease(new Vector2(999, 999));
                healthIncrease.IsLocked = false;
                healthIncrease.Level = 1;
                Game.Instance.GameState.PowerUps.Add(healthIncrease);
                Console.WriteLine("[PowerUp] Added and unlocked HealthIncrease for testing");
            }
            
            // Unlock any locked powerups for testing
            foreach (var powerUp in Game.Instance.GameState.PowerUps) {
                if (powerUp.IsLocked) {
                    powerUp.IsLocked = false;
                    powerUp.Level = 1;
                    Console.WriteLine($"[PowerUp] Unlocked {powerUp.GetType().Name} for testing");
                }
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
            int maxEnemies = 14 + (Core.Game.Instance.Score / 8);
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
            if (score <= 50)
                return random.Next(0, 2);
            else if (score <= 100)
                return random.Next(0, 3);
            else
                return random.Next(0, enemyControllers.Count);
        }

        private void SpawnPowerUps() {
            if (!Game.Instance.GameState.PowerUps.Any(p => !p.IsLocked))
            {
                Console.WriteLine("[PowerUp] No unlocked powerups available");
                return;
            }

            // Don't spawn if we already have too many powerups
            if (allPowerUps.Count >= MaxPowerUps)
            {
                Console.WriteLine($"[PowerUp] Max powerups reached ({allPowerUps.Count}/{MaxPowerUps})");
                return;
            }

            // Calculate base spawn rate based on difficulty
            float baseSpawnRate = CalculateBaseSpawnRate();
            
            Console.WriteLine($"[PowerUp] Spawn check - Rate: {baseSpawnRate:F3}, Current: {allPowerUps.Count}/{MaxPowerUps}");
            
            if (random.NextDouble() < baseSpawnRate) {
                Vector2 powerUpPosition = GetPowerUpSpawnPosition();
                PowerUp powerUpToSpawn = DeterminePowerUpToSpawn();
                
                if (powerUpToSpawn != null) {
                    powerUpToSpawn.transform.position = powerUpPosition;
                    Add_Game_Object((Game_Object)powerUpToSpawn);
                    allPowerUps.Add(powerUpToSpawn);
                    Console.WriteLine($"[PowerUp] Spawned {powerUpToSpawn.GetType().Name} at {powerUpPosition}");
                } else {
                    Console.WriteLine("[PowerUp] Failed to determine powerup to spawn");
                }
            } else {
                Console.WriteLine("[PowerUp] Spawn check failed (random roll)");
            }
        }

        private float CalculateBaseSpawnRate() {
            float playerHealth = Core.Game.Instance.player.health;
            float playerHealthRatio = playerHealth / Core.Game.Instance.player.health_max;
            int currentWave = Wave.currentWave;
            
            // Base spawn rate increases with difficulty and decreases with player health
            float baseRate = 0.05f; // 5% base chance
            
            // Increase spawn rate when player is low on health
            if (playerHealthRatio < 0.3f) {
                baseRate += 0.15f; // +15% when health < 30%
            } else if (playerHealthRatio < 0.5f) {
                baseRate += 0.10f; // +10% when health < 50%
            } else if (playerHealthRatio < 0.7f) {
                baseRate += 0.05f; // +5% when health < 70%
            }
            
            // Increase spawn rate with wave difficulty (but cap it)
            float waveBonus = Math.Min(currentWave * 0.01f, 0.10f); // Max 10% bonus from waves
            baseRate += waveBonus;
            
            // Cap the maximum spawn rate
            return Math.Min(baseRate, 0.25f); // Max 25% spawn rate
        }

        private Vector2 GetPowerUpSpawnPosition() {
            // Spawn powerups in a safe area away from edges
            float x = random.Next(-300, 300);
            float y = random.Next(-300, 300);
            return new Vector2(x, y);
        }

        private PowerUp DeterminePowerUpToSpawn() {
            var player = Core.Game.Instance.player;
            float currentTime = Game_Time.total;
            
            Console.WriteLine($"[PowerUp] Determining powerup to spawn - Health: {player.health}/{player.health_max} ({player.health/player.health_max:F2})");
            
            // Check if we should spawn health powerup
            if (ShouldSpawnHealthPowerup(currentTime)) {
                var healthPowerup = new HealthIncrease(Vector2.Zero);
                lastHealthPowerupSpawnTime = currentTime;
                consecutiveHealthPowerups++;
                Console.WriteLine($"[PowerUp] Selected HealthIncrease (consecutive: {consecutiveHealthPowerups})");
                return healthPowerup;
            }
            
            // Check if we should spawn speed powerup
            if (ShouldSpawnSpeedPowerup(currentTime)) {
                var speedPowerup = CreateSpeedPowerup();
                lastSpeedPowerupSpawnTime = currentTime;
                consecutiveHealthPowerups = 0; // Reset health powerup counter
                Console.WriteLine("[PowerUp] Selected SpeedBoost");
                return speedPowerup;
            }
            
            // Check if we should spawn fire rate powerup
            if (ShouldSpawnFireRatePowerup(currentTime)) {
                var fireRatePowerup = CreateFireRatePowerup();
                lastFireRatePowerupSpawnTime = currentTime;
                consecutiveHealthPowerups = 0; // Reset health powerup counter
                Console.WriteLine("[PowerUp] Selected FireRateBoost");
                return fireRatePowerup;
            }
            
            // If no specific powerup should spawn, randomly choose one (excluding health if we've spawned too many)
            var randomPowerup = GetRandomPowerup();
            if (randomPowerup != null) {
                Console.WriteLine($"[PowerUp] Selected random powerup: {randomPowerup.GetType().Name}");
            } else {
                Console.WriteLine("[PowerUp] No powerup selected");
            }
            return randomPowerup;
        }

        private bool ShouldSpawnHealthPowerup(float currentTime) {
            // Check if health powerup is unlocked
            if (!Game.Instance.GameState.PowerUps.Any(p => p.GetType() == typeof(HealthIncrease) && !p.IsLocked))
            {
                Console.WriteLine("[PowerUp] HealthIncrease not unlocked");
                return false;
            }
            
            // Check cooldown
            if (currentTime - lastHealthPowerupSpawnTime < HealthPowerupCooldown)
            {
                float remainingCooldown = HealthPowerupCooldown - (currentTime - lastHealthPowerupSpawnTime);
                Console.WriteLine($"[PowerUp] HealthIncrease on cooldown: {remainingCooldown:F1}s remaining");
                return false;
            }
            
            // More lenient consecutive spawn limit
            if (consecutiveHealthPowerups >= 3) // Increased from 2 to 3
            {
                Console.WriteLine($"[PowerUp] HealthIncrease consecutive limit reached: {consecutiveHealthPowerups}/3");
                return false;
            }
            
            var player = Core.Game.Instance.player;
            float healthRatio = player.health / player.health_max;
            
            // Much more generous health power-up spawning
            float spawnChance = 0.0f;
            if (healthRatio < 0.2f) spawnChance = 0.95f;      // 95% chance when health < 20%
            else if (healthRatio < 0.4f) spawnChance = 0.8f;  // 80% chance when health < 40%
            else if (healthRatio < 0.6f) spawnChance = 0.6f;  // 60% chance when health < 60%
            else if (healthRatio < 0.8f) spawnChance = 0.35f; // 35% chance when health < 80%
            else spawnChance = 0.15f;                         // 15% chance when health >= 80%
            
            bool shouldSpawn = random.NextDouble() < spawnChance;
            Console.WriteLine($"[PowerUp] HealthIncrease check - Health: {healthRatio:F2}, Chance: {spawnChance:F2}, Result: {shouldSpawn}");
            return shouldSpawn;
        }

        private bool ShouldSpawnSpeedPowerup(float currentTime) {
            // Check if speed powerup is unlocked
            if (!Game.Instance.GameState.PowerUps.Any(p => p.GetType() == typeof(SpeedBoost) && !p.IsLocked))
                return false;
            
            // Check cooldown
            if (currentTime - lastSpeedPowerupSpawnTime < SpeedPowerupCooldown)
                return false;
            
            // Higher chance for better gameplay
            float spawnChance = 0.45f; // 45% chance (was 30%)
            return random.NextDouble() < spawnChance;
        }

        private bool ShouldSpawnFireRatePowerup(float currentTime) {
            // Check if fire rate powerup is unlocked
            if (!Game.Instance.GameState.PowerUps.Any(p => p.GetType() == typeof(FireRateBoost) && !p.IsLocked))
                return false;
            
            // Check cooldown
            if (currentTime - lastFireRatePowerupSpawnTime < FireRatePowerupCooldown)
                return false;
            
            // Higher chance for better gameplay
            float spawnChance = 0.4f; // 40% chance (was 25%)
            return random.NextDouble() < spawnChance;
        }

        private PowerUp CreateSpeedPowerup() {
            var saveData = Game.Instance.GameState.PowerUpsSaveData.FirstOrDefault(p => p.PowerUpType == "SpeedBoost");
            if (saveData != null) {
                return new SpeedBoost(Vector2.Zero, saveData.SpeedBoost, saveData.Duration);
            }
            return new SpeedBoost(Vector2.Zero, 300f, 3f);
        }

        private PowerUp CreateFireRatePowerup() {
            var saveData = Game.Instance.GameState.PowerUpsSaveData.FirstOrDefault(p => p.PowerUpType == "FireRateBoost");
            if (saveData != null) {
                return new FireRateBoost(Vector2.Zero, saveData.FireDelayDecrease, saveData.Duration);
            }
            return new FireRateBoost(Vector2.Zero, 0.1f, 4f);
        }

        private PowerUp GetRandomPowerup() {
            var unlockedPowerUps = Game.Instance.GameState.PowerUps.Where(p => !p.IsLocked).ToList();
            if (unlockedPowerUps.Count == 0) return null;
            
            // Exclude health powerup if we've spawned too many recently
            var availablePowerUps = unlockedPowerUps;
            if (consecutiveHealthPowerups >= MaxConsecutiveHealthPowerups) {
                availablePowerUps = unlockedPowerUps.Where(p => p.GetType() != typeof(HealthIncrease)).ToList();
            }
            
            if (availablePowerUps.Count == 0) return null;
            
            var selectedPowerUp = availablePowerUps[random.Next(availablePowerUps.Count)];
            
            // Create the powerup based on type
            if (selectedPowerUp.GetType() == typeof(SpeedBoost)) {
                return CreateSpeedPowerup();
            } else if (selectedPowerUp.GetType() == typeof(FireRateBoost)) {
                return CreateFireRatePowerup();
            } else if (selectedPowerUp.GetType() == typeof(HealthIncrease)) {
                consecutiveHealthPowerups++;
                return new HealthIncrease(Vector2.Zero);
            }
            
            return null;
        }
    }
}
