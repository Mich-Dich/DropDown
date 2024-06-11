namespace Core.world
{
    using Core.defaults;
    using Newtonsoft.Json;

    public class GameState
    {
        public int AccountLevel { get; set; }
        public int AccountXP { get; set; }
        public int Currency { get; set; }
        public List<Ability> Abilities { get; set; } = new List<Ability>();

        public int XPForNextLevel()
        {
            return (int)(25 * Math.Pow(AccountLevel, 1.5));
        }

        public void AddXP(int amount)
        {
            AccountXP += amount;
            while (AccountXP >= XPForNextLevel())
            {
                AccountXP -= XPForNextLevel();
                AccountLevel++;
                Currency += CalculateCurrencyIncrease(AccountLevel);
            }
            GameStateManager.SaveGameState(this, "save.json");
        }

        public void IncreaseLevel()
        {
            AccountLevel++;
            AccountXP = 0;
            Currency += CalculateCurrencyIncrease(AccountLevel);
            GameStateManager.SaveGameState(this, "save.json");
        }

        public float GetXPProgress()
        {
            return (float)AccountXP / XPForNextLevel();
        }

        private int CalculateCurrencyIncrease(int level)
        {
            return (int)(10 * Math.Log(level + 1));
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
            string filePath = Path.Combine(saveFolderPath, fileName);
            var settings = new JsonSerializerSettings 
            { 
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Add this line
            };
            string json = JsonConvert.SerializeObject(gameState, Formatting.Indented, settings);

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, json);
            }
            else
            {
                Console.WriteLine($"File {filePath} already exists. Overwriting...");
                File.WriteAllText(filePath, json);
            }
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
            return loadedGameState;
        }
    }
}