namespace Hell
{
    internal class Programm
    {
        private static void Main(string[] args)
        {
            Core.Game game = new Game("Projektarbeit", 1600, 920);
            game.Run();
        }
    }
}
