namespace Projektarbeit.Levels
{
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.controller;

    public class Wave : Game_Object
    {
        public Wave(List<Spawner> spawners)
        :base(mobility: Mobility.STATIC)
        {
            this.spawners = spawners;
            TotalEnemiesInWave = CalculateTotalEnemies();
        }
     
        private bool Started = false;
        private bool Finished = false;
        private static List<Wave> waves;
        public static int currentWave {get; private set;} = 0;
        public static int TotalWaves => waves != null ? waves.Count : 0;
        
        private readonly List<Spawner> spawners;
        
        // Wave progress tracking
        public int TotalEnemiesInWave { get; private set; }
        public int EnemiesDefeated { get; set; } = 0;
        public float WaveProgress => TotalEnemiesInWave > 0 ? (float)EnemiesDefeated / TotalEnemiesInWave : 0f;
        private float stuckTimer = 0f;

        private int CalculateTotalEnemies()
        {
            int total = 0;
            foreach (var spawner in spawners)
            {
                total += spawner.MaxSpawn;
            }
            return total;
        }

        public void InitializeWave()
        {
            // Activate spawners
            foreach (Spawner spawner in spawners)
            {
                spawner.Active = true;
            }
            Started = true;
            EnemiesDefeated = 0;
            Game.Instance.get_active_map().all_game_objects.Add(this);
            
            // Debug output
            Console.WriteLine($"[Wave] Initialized: TotalEnemiesInWave={TotalEnemiesInWave}, Spawners={spawners.Count}");
        }

        public void RemoveWave()
        {
            Game.Instance.get_active_map().all_game_objects.Remove(this);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            // Safety check - if spawners list is null or empty, mark as finished
            if (spawners == null || spawners.Count == 0)
            {
                Console.WriteLine("[Wave] WARNING: No spawners in wave, marking as finished");
                Finished = true;
                return;
            }
            
            if (Finished)
            {
                // Only start next wave when all enemies are defeated
                if (EnemiesDefeated >= TotalEnemiesInWave)
                {
                    Console.WriteLine($"[Wave] All enemies defeated ({EnemiesDefeated}/{TotalEnemiesInWave}), starting next wave");
                    NextWave();
                }
                else
                {
                    // Safety timeout - if wave has been finished for too long but enemies aren't defeated, force completion
                    stuckTimer += deltaTime;
                    if (stuckTimer > 30f) // 30 second timeout
                    {
                        Console.WriteLine($"[Wave] WARNING: Wave stuck for {stuckTimer:F1}s at {EnemiesDefeated}/{TotalEnemiesInWave} - forcing completion");
                        EnemiesDefeated = TotalEnemiesInWave;
                        NextWave();
                    }
                }
                return;
            }

            // Update all spawners
            foreach (Spawner spawner in spawners)
            {
                if (spawner != null)
                {
                    spawner.Update(deltaTime);
                }
            }

            if (Started && !Finished)
            {
                // Check if any spawners are still active or in delay
                int activeSpawners = spawners.Count(s => s != null && s.Active);
                int delayedSpawners = spawners.Count(s => s != null && s.StartDelay > 0);
                
                // Mark wave as finished when all spawners are done spawning
                if (spawners.TrueForAll(spawner => spawner != null && spawner.Active == false))
                {
                    Finished = true;
                    stuckTimer = 0f; // Reset stuck timer when wave is marked as finished
                    Console.WriteLine($"[Wave] All spawners finished spawning. Waiting for {TotalEnemiesInWave - EnemiesDefeated} enemies to be defeated");
                    
                    // Add debug info about current state
                    Console.WriteLine($"[Wave] Debug - Active: {activeSpawners}, Delayed: {delayedSpawners}, Progress: {EnemiesDefeated}/{TotalEnemiesInWave}");
                    
                    // Safety check - if no enemies to defeat, immediately start next wave
                    if (TotalEnemiesInWave <= 0)
                    {
                        Console.WriteLine("[Wave] WARNING: No enemies in wave, immediately starting next wave");
                        NextWave();
                    }
                }
                else
                {
                    // Log active spawner status every few seconds when progress stalls
                    if (EnemiesDefeated > 0 && WaveProgress >= 0.8f) // At 80% or higher
                    {
                        Console.WriteLine($"[Wave] 80%+ DEBUG - Active: {activeSpawners}, Delayed: {delayedSpawners}, Progress: {EnemiesDefeated}/{TotalEnemiesInWave}");
                        for (int i = 0; i < spawners.Count; i++)
                        {
                            var spawner = spawners[i];
                            Console.WriteLine($"[Wave] Spawner {i}: Type={spawner.ControllerType.Name}, Active={spawner.Active}, Delay={spawner.StartDelay:F1}");
                        }
                    }
                }
            }
        }

        public static void NextWave()
        {
            // Safety check
            if (waves == null || currentWave >= waves.Count)
            {
                Console.WriteLine("[Wave] ERROR: NextWave called but waves list is invalid!");
                return;
            }

            waves[currentWave].RemoveWave();

            // Generate the next wave procedurally
            currentWave++; // Increment first
            int nextWaveNumber = currentWave + 1; // currentWave is 0-based, display is 1-based
            
            Console.WriteLine($"[Wave] Transitioning to wave {nextWaveNumber} (currentWave index: {currentWave})");
            
            GenerateProceduralWave(nextWaveNumber);
            
            // Safety check before initializing
            if (currentWave < waves.Count)
            {
                waves[currentWave].InitializeWave();
            }
            else
            {
                Console.WriteLine("[Wave] ERROR: Generated wave index out of bounds!");
            }
        }

        public static void LoadWaves()
        {
            /*
            Procedural wave generation system
            Generates waves dynamically and infinitely
            Wave patterns and scaling:
            - Early waves (1-10): Tutorial and introduction
            - Mid waves (11-50): Standard scaling with variety
            - Late waves (51+): Intense bullet hell with special patterns
            - Every 10th wave: Boss wave
            - Every 25th wave: Special challenge wave
            */
            currentWave = 0;
            waves = new List<Wave>();
            
            // Generate the first wave
            GenerateProceduralWave(1);
            
            // Initialize the first wave
            if (waves.Count > 0)
            {
                waves[0].InitializeWave();
                Console.WriteLine($"[Wave] First wave initialized with {waves[0].TotalEnemiesInWave} enemies");
            }
        }

        private static void GenerateProceduralWave(int waveNumber)
        {
            var newWaveSpawners = new List<Spawner>();
            
            // Determine wave type and difficulty
            WaveType waveType = DetermineWaveType(waveNumber);
            float difficultyMultiplier = CalculateDifficultyMultiplier(waveNumber);
            
            Console.WriteLine($"[Wave] Generating wave {waveNumber} - Type: {waveType}, Difficulty: {difficultyMultiplier:F2}x");
            
            switch (waveType)
            {
                case WaveType.Tutorial:
                    GenerateTutorialWave(newWaveSpawners, waveNumber);
                    break;
                case WaveType.Standard:
                    GenerateStandardWave(newWaveSpawners, waveNumber, difficultyMultiplier);
                    break;
                case WaveType.Boss:
                    GenerateBossWave(newWaveSpawners, waveNumber, difficultyMultiplier);
                    break;
                case WaveType.Challenge:
                    GenerateChallengeWave(newWaveSpawners, waveNumber, difficultyMultiplier);
                    break;
                case WaveType.BulletHell:
                    GenerateBulletHellWave(newWaveSpawners, waveNumber, difficultyMultiplier);
                    break;
            }
            
            waves.Add(new Wave(newWaveSpawners));
        }

        private enum WaveType
        {
            Tutorial,    // Waves 1-10: Learning the basics
            Standard,    // Normal waves with balanced scaling
            Boss,        // Every 10th wave: Boss with support
            Challenge,   // Every 25th wave: Special challenge
            BulletHell   // Waves 51+: Intense action
        }

        private static WaveType DetermineWaveType(int waveNumber)
        {
            if (waveNumber <= 4) return WaveType.Tutorial; // Tutorial up to wave 4
            if (waveNumber % 5 == 0) return WaveType.Boss; // Boss every 5th wave (5, 10, 15, 20, etc.)
            if (waveNumber % 25 == 0) return WaveType.Challenge;
            if (waveNumber >= 51) return WaveType.BulletHell;
            return WaveType.Standard;
        }

        private static float CalculateDifficultyMultiplier(int waveNumber)
        {
            // Exponential scaling with diminishing returns
            float baseMultiplier = 1.0f;
            float growthRate = 0.15f; // 15% increase per wave
            float maxMultiplier = 10.0f; // Cap at 10x difficulty
            
            float multiplier = baseMultiplier + (waveNumber - 1) * growthRate;
            return Math.Min(multiplier, maxMultiplier);
        }

        private static void GenerateTutorialWave(List<Spawner> spawners, int waveNumber)
        {
            // Tutorial waves: Fast-paced action from the start with better spawning
            // Note: Wave 5 and 10 are now handled by GenerateBossWave
            switch (waveNumber)
            {
                case 1: // 24 enemies - Much more action from the start
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 4, 3, 0, 1);
                    break;
                case 2: // 28 enemies - Add fewer tanks with faster spawning
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 4, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 4, 8, 2, 1);
                    break;
                case 3: // 30 enemies - Add snipers with more variety (matching old version)
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 5, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 4, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 6, 10, 10, 0);
                    break;
                case 4: // 45 enemies - More balanced variety
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 6, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 3, 8, 2, 1);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 3, 4, 4, 4, 1);
                    break;
                case 6: // 63 enemies - Explosive action intensifies
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 8, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 4, 6, 2, 1);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 3, 5, 4, 4, 1);
                    AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), 2, 3, 6, 6, 1);
                    break;
                case 7: // 72 enemies - Intense multi-enemy action
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 9, 3, 0, 0.5f);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 4, 6, 2, 0.5f);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 3, 6, 4, 4, 0.5f);
                    AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), 2, 3, 5, 6, 0.5f);
                    break;
                case 8: // 81 enemies - Bullet hell intensity with reasonable tanks
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 10, 2.5f, 0, 0.5f);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 5, 6, 2, 0.5f);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 4, 6, 3.5f, 4, 0.5f);
                    AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), 2, 4, 5, 6, 0.5f);
                    break;
                case 9: // 90 enemies - Maximum tutorial intensity with balanced tanks
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 11, 2.5f, 0, 0.5f);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 5, 6, 2, 0.5f);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 4, 7, 3.5f, 4, 0.5f);
                    AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), 3, 4, 4.5f, 6, 0.5f);
                    break;
                default:
                    // For any other tutorial wave numbers, generate a standard tutorial wave
                    Console.WriteLine($"[Wave] WARNING: Tutorial wave {waveNumber} not defined, generating default tutorial wave");
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 5, 4, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 3, 6, 2, 1);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 2, 4, 5, 4, 1);
                    break;
            }
        }

        private static void GenerateStandardWave(List<Spawner> spawners, int waveNumber, float difficultyMultiplier)
        {
            // Standard waves: Fast-paced action with better enemy distribution
            int baseEnemies = 30 + (waveNumber * 4); // More enemies per wave
            int swarmCount = Math.Min(4 + (waveNumber / 3), 8); // More spawners faster
            int tankCount = Math.Min(1 + (waveNumber / 6), 3);  // Fewer tanks (was 3+ up to 6)
            int sniperCount = Math.Min(2 + (waveNumber / 5), 6); // More snipers faster
            int explosiveCount = Math.Min(2 + (waveNumber / 6), 5);

            // Better enemy distribution with fewer tanks
            int swarmEnemies = (int)(baseEnemies * 0.5f * difficultyMultiplier); // More swarm
            int tankEnemies = (int)(baseEnemies * 0.15f * difficultyMultiplier); // Fewer tanks (was 0.25f)
            int sniperEnemies = (int)(baseEnemies * 0.25f * difficultyMultiplier); // More snipers (was 0.2f)
            int explosiveEnemies = (int)(baseEnemies * 0.1f * difficultyMultiplier);

            // Much faster spawning rates for engaging gameplay
            AddSpawnerGroup(spawners, typeof(SwarmEnemyController), swarmCount, 
                Math.Max(1, swarmEnemies / swarmCount), Math.Max(2, 4 - (waveNumber / 10)), 0, 0.5f);

            // Fewer tanks spawn at reasonable rates
            if (tankCount > 0 && tankEnemies > 0)
            {
                AddSpawnerGroup(spawners, typeof(TankEnemyController), tankCount, 
                    Math.Max(1, tankEnemies / tankCount), Math.Max(4, 8 - (waveNumber / 8)), 2, 1f);
            }

            // More snipers for variety
            AddSpawnerGroup(spawners, typeof(SniperEnemyController), sniperCount, 
                Math.Max(1, sniperEnemies / sniperCount), Math.Max(3, 5 - (waveNumber / 10)), 4, 0.5f);

            // Explosive enemies spawn consistently after tutorial
            if (waveNumber >= 5)
            {
                AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), explosiveCount, 
                    Math.Max(1, explosiveEnemies / explosiveCount), Math.Max(4, 6 - (waveNumber / 15)), 6, 1f);
            }
        }

        private static void GenerateBossWave(List<Spawner> spawners, int waveNumber, float difficultyMultiplier)
        {
            // Boss waves: ONLY the boss spawns (no other enemies during boss fight)
            int bossLevel = ((waveNumber - 5) / 5) + 1; // First boss at wave 5 = level 1, wave 10 = level 2, etc.
            
            Console.WriteLine($"[Wave] Generating Boss Wave {waveNumber} - Boss Level {bossLevel}");
            Console.WriteLine("[Wave] Boss fight initiated! No other enemies will spawn until boss is defeated.");
            
            // Use BossSpawner to ensure only one boss is spawned
            var bossSpawner = new BossSpawner(
                new Vector2(0, -600), // Center position
                typeof(BossController),
                1, // Only one boss
                0, // Immediate spawn
                0, // No delay
                false // Start inactive, will be activated by InitializeWave()
            );
            spawners.Add(bossSpawner);
            Console.WriteLine("[Wave] Created BossSpawner - will spawn exactly one boss");
        }

        private static void GenerateChallengeWave(List<Spawner> spawners, int waveNumber, float difficultyMultiplier)
        {
            // Challenge waves: Special patterns and intense action
            int challengeType = (waveNumber / 25) % 4; // 4 different challenge types
            
            switch (challengeType)
            {
                case 0: // Swarm rush
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 8, 8, 4, 0, 1f);
                    break;
                case 1: // Tank assault
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 6, 6, 8, 0, 2f);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 4, 4, 8, 10, 2f);
                    break;
                case 2: // Sniper barrage
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 8, 6, 6, 0, 2f);
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 4, 6, 5, 1f);
                    break;
                case 3: // Explosive chaos
                    AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), 6, 5, 8, 0, 3f);
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 5, 6, 5, 1f);
                    break;
            }
        }

        private static void GenerateBulletHellWave(List<Spawner> spawners, int waveNumber, float difficultyMultiplier)
        {
            // Bullet hell waves: Maximum intensity
            int baseEnemies = 30 + (waveNumber * 3);
            int swarmCount = 8;
            int tankCount = 6;
            int sniperCount = 5;
            int explosiveCount = 4;

            // Very fast spawning, many enemies
            AddSpawnerGroup(spawners, typeof(SwarmEnemyController), swarmCount, 
                baseEnemies / 4, 3, 0, 1f);
            AddSpawnerGroup(spawners, typeof(TankEnemyController), tankCount, 
                baseEnemies / 4, 6, 5, 1.5f);
            AddSpawnerGroup(spawners, typeof(SniperEnemyController), sniperCount, 
                baseEnemies / 4, 5, 10, 2f);
            AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), explosiveCount, 
                baseEnemies / 4, 6, 15, 2f);
        }

        private static void AddSpawnerGroup(List<Spawner> spawners, Type enemyType, int spawnerCount, 
            int enemiesPerSpawner, float spawnRate, float baseDelay, float delayIncrement)
        {
            Console.WriteLine($"[Wave] Adding {spawnerCount} spawners of {enemyType.Name}, {enemiesPerSpawner} enemies each = {spawnerCount * enemiesPerSpawner} total enemies");
            for (int i = 0; i < spawnerCount; i++)
            {
                float xPos = (i % 2 == 0 ? 1 : -1) * (600 - (i * 100));
                var spawner = new Spawner(
                    new Vector2(xPos, -600),
                    enemyType,
                    enemiesPerSpawner,
                    spawnRate,
                    baseDelay + (i * delayIncrement),
                    false // Start inactive, will be activated by InitializeWave()
                );
                spawners.Add(spawner);
                Console.WriteLine($"[Wave] Created spawner {i + 1}/{spawnerCount} - MaxSpawn: {spawner.MaxSpawn}, Delay: {baseDelay + (i * delayIncrement)}");
            }
        }

        public static Wave GetCurrentWave()
        {
            if (waves != null && currentWave < waves.Count)
            {
                return waves[currentWave];
            }
            return null;
        }

        public void EnemyDefeated()
        {
            EnemiesDefeated++;
            
            // Always log progress to help debug spawning issues
            float progress = WaveProgress * 100f;
            Console.WriteLine($"[Wave] Enemy defeated: {EnemiesDefeated}/{TotalEnemiesInWave} ({progress:F1}%) - Finished: {Finished}");
            
            // Note: NextWave() is called from the Update() method when all enemies are defeated
            // This prevents duplicate calls that could cause multiple wave transitions
        }
    }
}