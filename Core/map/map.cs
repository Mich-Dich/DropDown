using Core.game_objects;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Core {

    public class map {

        public List<game_object> all_game_objects { get; set; } = new List<game_object>();

        public map() { }

        public Vector2 loc_tile_size { get; set; } = new Vector2(100, 100);

        public int levelWidth { get; set; }
        public int levelHeight { get; set; }

        public struct tile_data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public tile_data(Int32 texture_slot, Matrix4 modle_matrix) {

                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        public void draw() {

            foreach(var tile in map_tiles.Values) {

                foreach(var sprite in tile.background) {
                    sprite.draw();
                }
            }

            for(int x = 0; x < backgound.Count; x++)
                backgound[x].draw();

            for(int x = 0; x < world.Count; x++)
                world[x].draw();
        }

        public void draw_denug() {

            debug_data.num_of_tiels_displayed = map_tiles.Count;

            for(int x = 0; x < world.Count; x++)
                world[x].draw_debug();
        }

        public virtual void draw_imgui() {

        }

        public void add_game_object(game_object game_object) {

            world.Add(game_object);
            all_game_objects.Add(game_object);
            Console.WriteLine($"Adding game_object [{game_object}] to world. Current count: {world.Count} ");
        }

        public void add_character(character character, Vector2? position = null) {

            world.Add(character);
            all_game_objects.Add(character);

            if(position != null)
                character.transform.position = position.Value;

            Console.WriteLine($"Adding character [{character}] to world. Current count: {world.Count} ");
        }

        public void add_sprite(sprite sprite) { backgound.Add(sprite); }

        public void add_sprite(world_layer world_layer, sprite sprite) {

            if(sprite == null)
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





        // ===============================================================================================================================================================
        // CURRENTLY IN DEV
        // ===============================================================================================================================================================

        public void add_background_sprite(sprite sprite, Vector2 position) {

            Vector2i key = new Vector2i((int)(position.X / tile_size), (int)(position.Y / tile_size));
            //Console.WriteLine($"tile key: {key}");
            if(!map_tiles.ContainsKey(key)) {

                map_tiles.Add(key, new map_tile());
                debug_data.num_of_tiels++;
            }

            map_tiles.TryGetValue(key, out map_tile current_tile);
            sprite.transform.position = position;
            current_tile.background.Add(sprite);
        }

        public void force_clear_map_tiles() {
            map_tiles.Clear();
        }

        // ===============================================================================================================================================================
        // CURRENTLY IN DEV
        // ===============================================================================================================================================================







        public map generate_backgound_tile(int width, int height) {

            // ------------------------ SETUP ------------------------
            Texture texture_atlas = resource_manager.get_texture("assets/textures/terrain.png", false);

            Random random = new Random();
            double missing_time_rate = 0f;

            float offset_x = (((float)width - 1) / 2) * loc_tile_size.X;
            float offset_y = (((float)height - 1) / 2) * loc_tile_size.Y;

            // Loop through the tiles and add them to the _positions list
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {

                    if(random.NextDouble() < missing_time_rate)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        continue;

                    transform loc_trans_buffer = new transform(new Vector2( x * loc_tile_size.X - offset_x, y * loc_tile_size.Y - offset_y),
                        new Vector2(loc_tile_size.X, loc_tile_size.Y),
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

        public void LoadLevel(string tmxFilePath, string tsxFilePath, string tilesetImageFilePath) {

            LevelData levelData = level_parser.ParseLevel(tmxFilePath, tsxFilePath);
            MapData mapData = levelData.Map;
            TilesetData tilesetData = levelData.Tileset;
            Texture tilesetTexture = resource_manager.get_texture(tilesetImageFilePath, false);

            int tilesetColumns = tilesetData.Columns;
            int tilesetRows = tilesetData.TileCount / tilesetColumns;
            int textureWidth = tilesetData.ImageWidth;
            int textureHeight = tilesetData.ImageHeight;

            for(int layerIndex = 0; layerIndex < mapData.Layers.Count; layerIndex++) {
                for(int y = 0; y < mapData.Height; y++) {
                    for(int x = 0; x < mapData.Width; x++) {

                        int tileGID = mapData.Layers[layerIndex].Tiles[x, y];
                        if(tileGID <= 0)
                            continue;

                        int tileIndex = tileGID - tilesetData.FirstGid;
                        if(tileIndex < 0)
                            continue;

                        int tileRow = tileIndex / tilesetColumns;
                        int tileColumn = tileIndex % tilesetColumns;

                        Console.WriteLine($"Tile GID: {tileGID}, Index: {tileIndex}, Row: {tileRow + 1}, Column: {tileColumn + 1}");

                        transform tileTransform = new transform(
                            new Vector2(x * mapData.TileWidth, y * mapData.TileHeight),
                            new Vector2(mapData.TileWidth, mapData.TileHeight),
                            0,
                            mobility.STATIC);

                        sprite tileSprite = new sprite(tileTransform, tilesetTexture)
                                        .select_texture_regionNew(tilesetColumns, tilesetRows, tileColumn, tileRow, tileGID, textureWidth, textureHeight);

                        if(layerIndex == 0)
                            backgound.Add(tileSprite);
                        else
                            world.Add(new game_object(tileTransform).set_sprite(tileSprite));
                    }
                }

            }
        }


        // ========================================== private ==========================================

        private List<sprite> backgound { get; set; } = new List<sprite>();       // change to list of lists => to reduce drawcalls
        private List<game_object> world { get; set; } = new List<game_object>();


        // ------------------------------------------ tiles ------------------------------------------
        private readonly int tile_size = 800;     // 8 default_sprites fit in one tile
        private Dictionary<Vector2i, map_tile> map_tiles { get; set; } = new Dictionary<Vector2i, map_tile>();

    }

    public struct map_tile {

        public Vector2              position = new Vector2();

        public List<sprite>         background = new List<sprite>();
        public List<game_object>    static_colliders = new List<game_object>();

        public map_tile() { }

    }

    public enum world_layer {

        None = 0,
        backgound = 1,      // will only be drawn (no collision)    eg. background image
        world = 2,          // can draw/interact/collide            eg. walls/chest
    }

}
