
namespace Core.defaults
{

    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using System.Reflection;

    public class MAP_default : Map {

        private const int DefaultCellSize = 100;
        private const string DefaultResourceName = "Core.defaults.textures.default_grid_bright.png";

        // Initializes a new instance of the <see cref="MAP_default"/> class.
        public MAP_default() {

            init_map_settings();
            generate_grid();
        }

        // Initializes the map settings.
        private void init_map_settings() {
            
            this.cellSize = DefaultCellSize;
            this.minDistancForCollision = this.cellSize * this.tileSize;
        }

        // Generates the grid with the specified dimensions.
        public void generate_grid(int sizeX = 10, int sizeY = 10) {
            
            var assembly = Assembly.GetExecutingAssembly();
            using(Stream stream = assembly.GetManifestResourceStream(DefaultResourceName)) {

                if(stream == null)
                    return;

                var texture = new Texture(stream);
                AddGridSprites(texture, sizeX, sizeY);
            }
        }

        // Adds grid sprites to the map.
        private void AddGridSprites(Texture texture, int sizeX, int sizeY) {
            
            for(int x = -(sizeX / 2) + 1; x < (sizeX / 2); x++) {
                for(int y = -(sizeY / 2) + 1; y < (sizeY / 2); y++) {
                    var position = new Vector2(x * cellSize, y * cellSize);
                    var sprite = new Core.world.Sprite(texture);
                    this.Add_Background_Sprite(sprite, position);
                }
            }
        }

    }
}
