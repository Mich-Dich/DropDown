namespace Projektarbeit
{
    internal class Programm
    {
        private static void Main(string[] args)
        {
            Core.Game game = new Game("DropDown", 1600, 920);
            game.Run();
        }
    }
}
