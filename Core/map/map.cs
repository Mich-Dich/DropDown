using Core.game_objects;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Core {

    public class map {

        public List<game_object> all_game_objects { get; set; } = new List<game_object>();

        public map() { }

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

            for(int x = 0; x < backgound.Count; x++)
                backgound[x].draw();

            for(int x = 0; x < world.Count; x++)
                world[x].draw();
        }

        public void draw_denug() {

            for(int x = 0; x < world.Count; x++)
                world[x].draw_debug();
        }

        public void add_game_object(game_object game_object) {

            world.Add(game_object);
            all_game_objects.Add(game_object);
            Console.WriteLine($"Adding game_object [{game_object}] to world. Current count: {world.Count} ");
        }

        public void add_character(character character) {

            world.Add(character);
            all_game_objects.Add(character);
            Console.WriteLine($"Adding character [{character}] to world. Current count: {world.Count} ");
        }

        public void add_sprite(sprite sprite) { backgound.Add(sprite); }

        public void add_sprite(world_layer world_layer, sprite sprite) {
            
            if (sprite == null)
                return;

            switch(world_layer) {
                case world_layer.None: break;
                case world_layer.world:
                    Console.WriteLine($"add_sprite() with argument [world_layer = world_layer.world] is not implemented yet");
                    //world.Add(new game_object().set_sprite(sprite));
                    break;
                        
                case world_layer.backgound: 
                    backgound.Add(sprite); 
                    break;
            }
                
        
        }

        public map generate_backgound_tile(int width, int height) {

            // ------------------------ SETUP ------------------------
            Texture texture_atlas = resource_manager.get_texture("assets/textures/terrain.png", false);
            
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
                        new Vector2(tile_size.X, tile_size.Y),
                        0, //(float)utility.degree_to_radians(_rotations[random.Next(0, 3)]),
                        mobility.STATIC);

                    // ============================ GRAS FILD ============================ 
                    if(random.NextDouble() < 0.01f)
                        backgound.Add(new sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 3, 30));
                    else if(random.NextDouble() < 0.03f)
                        backgound.Add(new sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 5, 26));
                    else if(random.NextDouble() < 0.1f)
                        backgound.Add(new sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 10, 5));
                    else
                        backgound.Add(new sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 4, 28));
                }
            }

            return this;
        }

        // ========================================== private ==========================================

        private List<sprite> backgound { get; set; } = new List<sprite>();       // change to list of lists => to reduce drawcalls
        private List<game_object> world { get; set; } = new List<game_object>();

    }

    public struct map_tile {

        public map_tile(Texture texture) {
         
            Texture = texture;
        }

        public Texture Texture { get; set; }
        // TODO: add more data and ruls for map generation

    }

    public enum world_layer {

        None = 0,
        backgound = 1,      // will only be drawn (no collision)    eg. background image
        world = 2,          // can draw/interact/collide            eg. walls/chest
    }

}
