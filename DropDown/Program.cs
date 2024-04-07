using Core;

namespace DropDown {

    public class programm {

        public static void Main(string[] args) {

            game game = new generate_map("Texture_test", 1800, 900);
            game.run();
        }
    }
}