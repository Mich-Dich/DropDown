using Core.game_objects;
using Core.visual;
using OpenTK.Mathematics;

namespace Core {

    public class map {

        public List<game_object> all_game_objects {  get; set; } = new List<game_object>();

        public map() {

            init();
        }

        public Vector2 tile_size { get; set; } = new Vector2(100, 100);

        public struct tile_data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public tile_data(Int32 texture_slot, Matrix4 modle_matrix) {

                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        public void draw() {

            for(int x = 0; x < floor.Count; x++)
                floor[x].draw();
        }

        public map generate_square(int width, int height) {

            // ------------------------ SETUP ------------------------
            SpriteBatch sprite_batch = game.instance.ResourceManager.GetSpriteBatch("assets/textures", false);
            map_tiles.Add(new map_tile(game.instance.ResourceManager.GetTexture("assets/textures/floor_tile_00.png", false)));
            map_tiles.Add(new map_tile(game.instance.ResourceManager.GetTexture("assets/textures/floor_tile_03.png", false)));

            Random random = new Random();
            double missing_time_rate = 0f;

            float offset_x = (((float)width - 1) / 2) * tile_size.X;
            float offset_y = (((float)height - 1) / 2) * tile_size.Y;

            // Loop through the tiles and add them to the _positions list
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {

                    if(random.NextDouble() < missing_time_rate)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        continue;

                    transform loc_trans_buffer = new transform(new Vector2( x * tile_size.X - offset_x, y * tile_size.Y - offset_y),
                        new Vector2(tile_size.X / 2, tile_size.Y / 2),
                        (float)utility.degree_to_radians(_rotations[random.Next(0, 3)]),
                        mobility.STATIC);

                    if(random.NextDouble() < 0.5f)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        floor.Add(new sprite(loc_trans_buffer, map_tiles[0].Texture));
                    else
                        floor.Add(new sprite(loc_trans_buffer, map_tiles[1].Texture));
                }
            }

            return this;
        }

        // ========================================== private ==========================================

        //private sprite _core_sprite = new sprite(Vector2.Zero, new Vector2(100), 0, mobility.STATIC);
        //private List<tile_data> _tile_data = new List<tile_data>{};
        //private List<Matrix4> _tile_modle_matrix = new List<Matrix4>();

        private readonly float[] _rotations = { 0, 90, 180 ,270 };
        private List<sprite> floor = new List<sprite>();
        private SpriteBatch sprite_batch { get; set; }

        private List<map_tile> map_tiles { get; set; } = new();

        private void init() { }

    }

    public struct map_tile {

        public map_tile(Texture texture) {
         
            Texture = texture;
        }

        public Texture Texture { get; set; }
        // TODO: add more data and ruls for map generation

    }

}
