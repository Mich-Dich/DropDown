namespace Core.util
{
    using Core;
    using Core.defaults;
    using Newtonsoft.Json;

    public class GameState
    {
        public int AccountLevel { get; set; }
        public int AccountXP { get; set; }
        public int Currency { get; set; }
        [JsonIgnore]
        public List<Ability> Abilities { get; set; } = new List<Ability>();
        [JsonIgnore]
        public List<PowerUp> PowerUps { get; set; } = new List<PowerUp>();
        public List<PowerUpSaveData> PowerUpsSaveData { get; set; } = new List<PowerUpSaveData>();
        public List<AbilitySaveData> AbilitiesSaveData { get; set; } = new List<AbilitySaveData>();
        
        // Performance optimization: Batched save system
        [JsonIgnore]
        private float lastSaveTime = 0f;
        [JsonIgnore]
        private const float SaveCooldown = 5.0f; // Save every 5 seconds max
        [JsonIgnore]
        private bool needsSave = false;

        public int XPForNextLevel()
        {
            return (int)(25 * Math.Pow(AccountLevel, 1.5));
        }

        public void AddXP(int amount)
        {
            AccountXP += amount;
            bool leveledUp = false;
            while (AccountXP >= XPForNextLevel())
            {
                AccountXP -= XPForNextLevel();
                AccountLevel++;
                Currency += CalculateCurrencyIncrease(AccountLevel);
                leveledUp = true;

                // Call PlayerLevelUp when the player levels up
                Game.Instance.get_active_map().PlayerLevelUp();
            }
            
            // Mark that we need to save, but don't save immediately for performance
            needsSave = true;
            
            // Only save immediately on level up (important milestone)
            if (leveledUp)
            {
                GameStateManager.SaveGameState(this, "save.json");
                lastSaveTime = Game_Time.total;
                needsSave = false;
            }
        }

        public void IncreaseLevel()
        {
            AccountLevel++;
            AccountXP = 0;
            Currency += CalculateCurrencyIncrease(AccountLevel);

            Game.Instance.get_active_map().PlayerLevelUp();

            // Save immediately on manual level increase (important milestone)
            GameStateManager.SaveGameState(this, "save.json");
            lastSaveTime = Game_Time.total;
            needsSave = false;
        }

        public float GetXPProgress()
        {
            return (float)AccountXP / XPForNextLevel();
        }

        private int CalculateCurrencyIncrease(int level)
        {
            return (int)(10 * Math.Log(level + 1));
        }

        public void SetPlayerAbility()
        {
            foreach (var ability in Abilities)
            {
                if (ability.IsEquipped)
                {
                    Game.Instance.player.Ability = ability;
                    break;
                }
            }
        }

        public void RemoveDuplicatePowerUps()
        {
            var groupedPowerUps = PowerUps.GroupBy(p => p.GetType()).ToList();
            PowerUps.Clear();

            foreach (var group in groupedPowerUps)
            {
                var highestLevelPowerUp = group.OrderByDescending(p => p.Level).First();
                PowerUps.Add(highestLevelPowerUp);
            }
        }
        
        // Performance optimization: Periodic save system
        public void UpdatePeriodicSave()
        {
            if (needsSave && (Game_Time.total - lastSaveTime) >= SaveCooldown)
            {
                GameStateManager.SaveGameState(this, "save.json");
                lastSaveTime = Game_Time.total;
                needsSave = false;
                // Remove console spam - only log periodic saves in debug mode
                #if DEBUG
                Console.WriteLine($"[GameState] Periodic save at {Game_Time.total:F1}s");
                #endif
            }
        }
        
        // Force save (for important events like game exit)
        public void ForceSave()
        {
            if (needsSave)
            {
                GameStateManager.SaveGameState(this, "save.json");
                lastSaveTime = Game_Time.total;
                needsSave = false;
            }
        }
    }

    public static class GameStateManager
    {
        private static string saveFolderPath;

        static GameStateManager()
        {
            string appDataFolder;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else
            {
                appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            saveFolderPath = Path.Combine(appDataFolder, "Projektarbeit", "GameSaves");

            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }

            Console.WriteLine($"Save folder path: {saveFolderPath}");
        }

        public static void SaveGameState(GameState gameState, string fileName)
        {
            gameState.PowerUpsSaveData.Clear();
            gameState.AbilitiesSaveData.Clear();
            foreach (var powerUp in gameState.PowerUps)
            {
                gameState.PowerUpsSaveData.Add(powerUp.ToSaveData());
            }

            foreach (var ability in gameState.Abilities)
            {
                gameState.AbilitiesSaveData.Add(ability.ToSaveData());
            }

            string filePath = Path.Combine(saveFolderPath, fileName);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(gameState, Formatting.Indented, settings);

            // Remove performance-killing console logging - just write the file
            File.WriteAllText(filePath, json);
        }

        public static GameState? LoadGameState(string fileName)
        {
            string filePath = Path.Combine(saveFolderPath, fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist. Creating a new one...");
                var newGameState = new GameState
                {
                    AccountLevel = 1,
                    AccountXP = 0,
                    Currency = 0
                };
                SaveGameState(newGameState, fileName);
                return newGameState;
            }

            string json = File.ReadAllText(filePath);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            GameState loadedGameState = JsonConvert.DeserializeObject<GameState>(json, settings);

            Console.WriteLine("Loaded PowerUpSaveData: " + loadedGameState.PowerUpsSaveData.Count);

            loadedGameState.PowerUps = Game.Instance.loadPowerups(loadedGameState.PowerUpsSaveData);

            Console.WriteLine("Loaded Projectiles:" + loadedGameState.PowerUps.Count);

            Console.WriteLine("Loaded AbilitySaveData: " + loadedGameState.AbilitiesSaveData.Count);

            loadedGameState.Abilities = Game.Instance.loadAbilities(loadedGameState.AbilitiesSaveData);

            Console.WriteLine("Loaded Abilities:" + loadedGameState.Abilities.Count);

            loadedGameState?.SetPlayerAbility();

            return loadedGameState;
        }
    }
}