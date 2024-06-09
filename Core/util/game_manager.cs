using Newtonsoft.Json;
using System;
using System.IO;

namespace Core.world
{
    public class GameState
    {
        public int AccountLevel { get; set; }
        public int AccountXP { get; set; }
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
            string json = JsonConvert.SerializeObject(gameState, Formatting.Indented);

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
                SaveGameState(new GameState(), fileName);
            }

            string json = File.ReadAllText(filePath);
            GameState loadedGameState = JsonConvert.DeserializeObject<GameState>(json);
            return loadedGameState;
        }
    }
}