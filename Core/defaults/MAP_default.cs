using Core.render;
using System.Reflection;
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
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Core.defaults.textures.default_grid_bright.png";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return;
                var texture = new Texture(stream);

                for(int x = -(size_x/2)+1; x < (size_x / 2); x++) {
                    for(int y = -(size_y / 2)+1; y < (size_y / 2); y++) {
                        this.Add_Background_Sprite(
                            new Core.world.Sprite(texture),
                            new OpenTK.Mathematics.Vector2(x * cellSize, y * cellSize));
                    }
                }
            }
        }

    }
}
