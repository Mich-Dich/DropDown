
namespace Projektarbeit {

    internal class Programm {

        private static void Main(string[] args) {

            Core.Game game = new Game("Texture_test", 1600, 920);
            game.Run();
        }
    }
}
