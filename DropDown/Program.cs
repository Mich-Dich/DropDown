using Core;

namespace DropDown {

    public class programm {

        public static void Main(string[] args) {

            //game game = new drop_down("drop_down", 800, 600);
            //game.run();

            //game game = new texture_test("Texture_test", 1800, 900);
            //game.run();

            //game game = new buffer_abstraction("Texture_test", 1800, 900);
            //game.run();

            game game = new add_input("Texture_test", 1800, 900);
            game.run();
        }
    }
}