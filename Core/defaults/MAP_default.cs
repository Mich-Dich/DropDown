
namespace DropDown {
    using Core.util;
    using Core.world.map;

    public class MAP_default : Map {

        public MAP_default() {

            this.cellSize = 100;
            this.minDistancForCollision = (float)(this.cellSize * this.tileSize);
            generate_grid();
        }

        public void generate_grid(int size_x = 10, int size_y = 10) {

            for(int x = -(size_x/2)+1; x < (size_x / 2); x++) {
                for(int y = -(size_y / 2)+1; y < (size_y / 2); y++) {

                    this.Add_Background_Sprite(
                        new Core.world.Sprite(Resource_Manager.Get_Texture("defaults/textures/default_grid_bright.png")),
                        new OpenTK.Mathematics.Vector2(x * cellSize, y * cellSize));
                }
            }

        }

    }
}
