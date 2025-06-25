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
            if (Finished)
            {
                // Only start next wave when all enemies are defeated
                if (EnemiesDefeated >= TotalEnemiesInWave)
                {
                    Console.WriteLine($"[Wave] All enemies defeated ({EnemiesDefeated}/{TotalEnemiesInWave}), starting next wave");
                    NextWave();
                }
                return;
            }

            foreach (Spawner spawner in spawners)
            {
                spawner.Update(deltaTime);
            }

            if (Started && !Finished)
            {
                // Mark wave as finished when all spawners are done spawning
                if (spawners.TrueForAll(spawner => spawner.Active == false))
                {
                    Finished = true;
                    Console.WriteLine($"[Wave] All spawners finished spawning. Waiting for {TotalEnemiesInWave - EnemiesDefeated} enemies to be defeated");
                }
            }
        }

        public static void NextWave()
        {
            waves[currentWave].RemoveWave();

            // Generate the next wave procedurally
            int nextWaveNumber = currentWave + 2; // +2 because currentWave is 0-based and we want the next wave number
            GenerateProceduralWave(nextWaveNumber);
            currentWave++;
            waves[currentWave].InitializeWave();
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
            if (waveNumber <= 7) return WaveType.Tutorial; // Tutorial up to wave 7
            if (waveNumber == 8 || (waveNumber > 8 && (waveNumber - 8) % 10 == 0)) return WaveType.Boss; // Boss at wave 8, then every 10th wave after (18, 28, 38, etc.)
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
            switch (waveNumber)
            {
                case 1: // 24 enemies - Much more action from the start
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 4, 3, 0, 1);
                    break;
                case 2: // 28 enemies - Add fewer tanks with faster spawning
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 4, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 4, 8, 2, 1);
                    break;
                case 3: // 36 enemies - Add snipers with balanced tanks
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 5, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 3, 8, 2, 1);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 3, 3, 4, 4, 1);
                    break;
                case 4: // 45 enemies - More balanced variety
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 6, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 3, 8, 2, 1);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 3, 4, 4, 4, 1);
                    break;
                case 5: // 54 enemies - Add explosive enemies with fast action
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 6, 7, 3, 0, 1);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 3, 8, 2, 1);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 3, 4, 4, 4, 1);
                    AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), 2, 3, 6, 6, 1);
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
                case 10: // Boss wave - ONLY BOSS (no support)
                    AddSpawnerGroup(spawners, typeof(BossController), 1, 1, 20, 0, 0);
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
            int bossLevel = waveNumber == 8 ? 1 : ((waveNumber - 8) / 10) + 1; // First boss at wave 8 = level 1, then increment every 10 waves
            
            Console.WriteLine($"[Wave] Generating Boss Wave {waveNumber} - Boss Level {bossLevel}");
            Console.WriteLine("[Wave] Boss fight initiated! No other enemies will spawn until boss is defeated.");
            
            // Add only the boss - no support enemies during boss fight for focused gameplay
            AddSpawnerGroup(spawners, typeof(BossController), 1, 1, 30, 0, 0);
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
            for (int i = 0; i < spawnerCount; i++)
            {
                float xPos = (i % 2 == 0 ? 1 : -1) * (600 - (i * 100));
                spawners.Add(new Spawner(
                    new Vector2(xPos, -600),
                    enemyType,
                    enemiesPerSpawner,
                    spawnRate,
                    baseDelay + (i * delayIncrement),
                    false // Start inactive, will be activated by InitializeWave()
                ));
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
            // Remove performance-killing console logging - this was called every enemy death!
            #if DEBUG
            // Only log milestone progress in debug mode
            if (EnemiesDefeated % 5 == 0 || EnemiesDefeated == TotalEnemiesInWave)
            {
                float progress = WaveProgress * 100f;
                Console.WriteLine($"[Wave] Enemy defeated: {EnemiesDefeated}/{TotalEnemiesInWave} ({progress:F1}%)");
            }
            #endif
            
            // Check if wave is complete
            if (EnemiesDefeated >= TotalEnemiesInWave && Finished)
            {
                Console.WriteLine($"[Wave] Wave complete! All {TotalEnemiesInWave} enemies defeated.");
            }
        }
    }
}