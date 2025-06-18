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
            if (waveNumber <= 10) return WaveType.Tutorial;
            if (waveNumber % 10 == 0) return WaveType.Boss;
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
            // Tutorial waves: Introduce mechanics gradually with more action
            switch (waveNumber)
            {
                case 1: // 15 enemies - More action from the start
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 4, 6, 0, 2);
                    break;
                case 2: // 22 enemies - Add tanks with more swarm
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 4, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 6, 12, 8, 0);
                    break;
                case 3: // 30 enemies - Add snipers with more variety
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 5, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 4, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 6, 10, 10, 0);
                    break;
                case 4: // 38 enemies - More variety and intensity
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 6, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 5, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 4, 10, 10, 0);
                    break;
                case 5: // 45 enemies - Add explosive enemies with more action
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 7, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 5, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 5, 10, 10, 0);
                    break;
                case 6: // 52 enemies - More explosive action
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 8, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 6, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 5, 10, 10, 0);
                    break;
                case 7: // 60 enemies - Intense action
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 9, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 6, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 6, 10, 10, 0);
                    break;
                case 8: // 68 enemies - Bullet hell begins
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 10, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 7, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 6, 10, 10, 0);
                    break;
                case 9: // 75 enemies - Maximum intensity
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 4, 11, 6, 0, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 1, 7, 12, 8, 0);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 1, 7, 10, 10, 0);
                    break;
                case 10: // Boss wave with support
                    AddSpawnerGroup(spawners, typeof(BossController), 1, 1, 20, 0, 0);
                    AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 2, 4, 6, 3, 2);
                    AddSpawnerGroup(spawners, typeof(SniperEnemyController), 2, 3, 10, 8, 2);
                    AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, 3, 12, 15, 2);
                    break;
            }
        }

        private static void GenerateStandardWave(List<Spawner> spawners, int waveNumber, float difficultyMultiplier)
        {
            // Standard waves: Balanced scaling with variety - more aggressive scaling
            int baseEnemies = 20 + (waveNumber * 3); // Increased base enemies and scaling
            int swarmCount = Math.Min(3 + (waveNumber / 4), 8); // More spawners faster
            int tankCount = Math.Min(2 + (waveNumber / 6), 6);  // More tanks faster
            int sniperCount = Math.Min(1 + (waveNumber / 8), 5); // More snipers faster
            int explosiveCount = Math.Min(1 + (waveNumber / 10), 4);

            // Distribute enemies based on wave characteristics - more aggressive distribution
            int swarmEnemies = (int)(baseEnemies * 0.5f * difficultyMultiplier);
            int tankEnemies = (int)(baseEnemies * 0.25f * difficultyMultiplier);
            int sniperEnemies = (int)(baseEnemies * 0.15f * difficultyMultiplier);
            int explosiveEnemies = (int)(baseEnemies * 0.1f * difficultyMultiplier);

            // Add swarm enemies - faster spawning
            AddSpawnerGroup(spawners, typeof(SwarmEnemyController), swarmCount, 
                swarmEnemies / swarmCount, Math.Max(3, 7 - (waveNumber / 8)), 0, 1.5f);

            // Add tank enemies - faster spawning
            AddSpawnerGroup(spawners, typeof(TankEnemyController), tankCount, 
                tankEnemies / tankCount, Math.Max(6, 14 - (waveNumber / 12)), 5, 2f);

            // Add sniper enemies - faster spawning
            AddSpawnerGroup(spawners, typeof(SniperEnemyController), sniperCount, 
                sniperEnemies / sniperCount, Math.Max(5, 11 - (waveNumber / 15)), 10, 3f);

            // Add explosive enemies (every 2nd wave) - faster spawning
            if (waveNumber % 2 == 0)
            {
                AddSpawnerGroup(spawners, typeof(ExplosivEnemyController), explosiveCount, 
                    explosiveEnemies / explosiveCount, Math.Max(6, 11 - (waveNumber / 20)), 15, 4f);
            }
        }

        private static void GenerateBossWave(List<Spawner> spawners, int waveNumber, float difficultyMultiplier)
        {
            // Boss waves: Boss with support enemies
            int bossLevel = waveNumber / 10;
            
            // Add boss
            AddSpawnerGroup(spawners, typeof(BossController), 1, 1, 20, 0, 0);
            
            // Add support enemies based on boss level
            int supportEnemies = 10 + (bossLevel * 5);
            int swarmSupport = (int)(supportEnemies * 0.4f * difficultyMultiplier);
            int sniperSupport = (int)(supportEnemies * 0.3f * difficultyMultiplier);
            int tankSupport = (int)(supportEnemies * 0.3f * difficultyMultiplier);

            AddSpawnerGroup(spawners, typeof(SwarmEnemyController), 2, swarmSupport / 2, 6, 3, 2);
            AddSpawnerGroup(spawners, typeof(SniperEnemyController), 2, sniperSupport / 2, 10, 8, 2);
            AddSpawnerGroup(spawners, typeof(TankEnemyController), 2, tankSupport / 2, 12, 15, 2);
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
            float progress = WaveProgress * 100f;
            Console.WriteLine($"[Wave] Enemy defeated: {EnemiesDefeated}/{TotalEnemiesInWave} ({progress:F1}%)");
            
            // Check if wave is complete
            if (EnemiesDefeated >= TotalEnemiesInWave && Finished)
            {
                Console.WriteLine($"[Wave] Wave complete! All {TotalEnemiesInWave} enemies defeated.");
            }
        }
    }
}