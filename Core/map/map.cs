using Core.game_objects;
using Core.util;
using Core.visual;
using Core.physics;
using OpenTK.Mathematics;

namespace Core {

    public class map {

        public List<game_object> all_dynamic_game_objects { get; set; } = new List<game_object>();

        public map() {}

        public int levelWidth { get; set; }
        public int levelHeight { get; set; }
        public int tileHeight { get; set; }
        public int tileWidth { get; set; }
        public int TilesOnScreenWidth { get; set; }
        public int TilesOnScreenHeight { get; set; }


        public struct tile_data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public tile_data(Int32 texture_slot, Matrix4 modle_matrix) {

                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        public void draw() {

            Vector2 camera_pos = game.instance.camera.transform.position;
            Vector2 camera_size = game.instance.camera.get_view_size_in_world_coord() + new Vector2(300);

            float tiel_size = tile_size * cell_size;

            foreach (var tile in map_tiles) {

                float overlapX = (camera_size.X / 2 + tiel_size / 2) - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = (camera_size.Y / 2 + tiel_size / 2) - Math.Abs(camera_pos.Y - tile.Key.Y);

                if(overlapX > 0 && overlapY > 0) {

                    debug_data.num_of_tiels_displayed++;
                    foreach(var sprite in tile.Value.background) {
                        sprite.draw();
                    }
                }
            }

            for(int x = 0; x < backgound.Count; x++)
                backgound[x].draw();

            for(int x = 0; x < world.Count; x++)
                world[x].draw();
        }

        public void draw_denug() {

            for(int x = 0; x < world.Count; x++)
                world[x].draw_debug();


            foreach(var tile in map_tiles.Values) {

                foreach(var game_object in tile.static_game_object) {
                    game_object.draw_debug();
                }
            }

        }

        public virtual void draw_imgui() {

        }

        public void add_game_object(game_object game_object) {

            world.Add(game_object);
            all_dynamic_game_objects.Add(game_object);
            //Console.WriteLine($"Adding game_object [{game_object}] to world. Current count: {world.Count} ");
        }

        public void add_character(character character, Vector2? position = null) {

            world.Add(character);
            all_dynamic_game_objects.Add(character);

            if(position != null)
                character.transform.position = position.Value;

            //Console.WriteLine($"Adding character [{character}] to world. Current count: {world.Count} ");
        }

        public void add_sprite(sprite sprite) { backgound.Add(sprite); }

        public void add_sprite(world_layer world_layer, sprite sprite) {

            if(sprite == null)
                return;

            switch(world_layer) {
            case world_layer.None: break;
            case world_layer.world:
            //Console.WriteLine($"add_sprite() with argument [world_layer = world_layer.world] is not implemented yet");
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

        public void add_background_sprite(sprite sprite, Vector2 position, bool use_cell_size = true) {

            var current_tile= get_correct_map_tile(position);

            sprite.transform.position = position;
            if(use_cell_size)
                sprite.transform.size = new Vector2(cell_size);

            current_tile.background.Add(sprite);
        }

        public void add_static_game_object(game_object new_game_object, Vector2 position, bool use_cell_size = true) {

            var current_tile= get_correct_map_tile(position);

            new_game_object.transform.position = position;
            new_game_object.transform.mobility = mobility.STATIC;
            if(new_game_object.collider != null)
                new_game_object.collider.offset.mobility = mobility.STATIC;
            if(use_cell_size)
                new_game_object.transform.size = new Vector2(cell_size);

            current_tile.static_game_object.Add(new_game_object);
        }

        private map_tile get_correct_map_tile(Vector2 position) {

            int final_tile_size = (tile_size * cell_size);
            Vector2i key = new Vector2i((int)Math.Floor(position.X / final_tile_size), (int)Math.Floor(position.Y / final_tile_size));
            key *= final_tile_size;
            key += new Vector2i(final_tile_size / 2, (final_tile_size / 3));

            Console.WriteLine($"position => key      {position} => {key}");

            if (!map_tiles.ContainsKey(key)) {

                map_tiles.Add(key, new map_tile(key));
                debug_data.num_of_tiels++;

                Console.WriteLine($"-----------  add tile at: {key}");
            }

            map_tiles.TryGetValue(key, out map_tile current_tile);
            return current_tile;
        }

        public void force_clear_map_tiles() {
            map_tiles.Clear();
            debug_data.num_of_tiels = 0;
        }

        // ===============================================================================================================================================================
        // CURRENTLY IN DEV
        // ===============================================================================================================================================================







        public map generate_backgound_tile(int width, int height) {

            // ------------------------ SETUP ------------------------
            Texture texture_atlas = resource_manager.get_texture("assets/textures/terrain.png", false);

            Random random = new Random();
            double missing_time_rate = 0f;

            float offset_x = (((float)width - 1) / 2) * cell_size;
            float offset_y = (((float)height - 1) / 2) * cell_size;

            // Loop through the tiles and add them to the _positions list
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {

                    if(random.NextDouble() < missing_time_rate)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        continue;

                    transform loc_trans_buffer = new transform(new Vector2( x * cell_size - offset_x, y * cell_size - offset_y),
                        new Vector2(cell_size),
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
            this.levelWidth = mapData.LevelPixelWidth;
            this.levelHeight = mapData.LevelPixelHeight;
            this.tileWidth = mapData.TileWidth;
            this.tileHeight = mapData.TileHeight;
            TilesetData tilesetData = levelData.Tileset;
            Texture tilesetTexture = resource_manager.get_texture(tilesetImageFilePath, false);

            int tilesetColumns = tilesetData.Columns;
            int tilesetRows = tilesetData.TileCount / tilesetColumns;
            int textureWidth = tilesetData.ImageWidth;
            int textureHeight = tilesetData.ImageHeight;

            for (int layerIndex = 0; layerIndex < mapData.Layers.Count; layerIndex++) {
                for (int y = 0; y < mapData.Height; y++) {
                    for (int x = 0; x < mapData.Width; x++) {
                        int tileGID = mapData.Layers[layerIndex].Tiles[x, y];
                        if (tileGID <= 0)
                            continue;

                        int tileIndex = tileGID - tilesetData.FirstGid;
                        if (tileIndex < 0)
                            continue;

                        int tileRow = tileIndex / tilesetColumns;
                        int tileColumn = tileIndex % tilesetColumns;

                        transform tileTransform = new transform(
                            new Vector2(x * mapData.TileWidth, y * mapData.TileHeight),
                            new Vector2(mapData.TileWidth, mapData.TileHeight),
                            0,
                            mobility.STATIC);

                        sprite tileSprite = new sprite(tileTransform, tilesetTexture)
                                            .select_texture_regionNew(tilesetColumns, tilesetRows, tileColumn, tileRow, tileGID, textureWidth, textureHeight);

                        if (tilesetData.CollidableTiles.ContainsKey(tileIndex) && tilesetData.CollidableTiles[tileIndex]) {
                            transform buffer = ((tileColumn == 0 && tileRow == 5))
                                ? new transform(new Vector2(0, -10), new Vector2(0, -23), 0, mobility.STATIC)
                                : new transform(Vector2.Zero, Vector2.Zero, 0, mobility.STATIC);

                            game_object newGameObject = new game_object(tileTransform)
                                .set_sprite(tileSprite)
                                .add_collider(new collider(collision_shape.Square) { Blocking = true }.set_offset(buffer))
                                .set_mobility(mobility.STATIC);

                            add_static_game_object(newGameObject, tileTransform.position);

                            if(layerIndex == 0)
                                add_background_sprite(tileSprite, tileTransform.position);

                        } else {
                            if (layerIndex == 0)
                                add_background_sprite(tileSprite, tileTransform.position);
                            else
                                all_dynamic_game_objects.Add(new game_object(tileTransform).set_sprite(tileSprite));
                        }
                    }
                }
            }
        }


        // ========================================== private ==========================================

        private List<sprite> backgound { get; set; } = new List<sprite>();       // change to list of lists => to reduce drawcalls
        private List<game_object> world { get; set; } = new List<game_object>();


        // ------------------------------------------ tiles ------------------------------------------
        protected int cell_size = 200;
        protected int tile_size = 8;     // 8 default_sprites fit in one tile
        private Dictionary<Vector2i, map_tile> map_tiles { get; set; } = new Dictionary<Vector2i, map_tile>();

    }

    public struct map_tile {

        public Vector2              position = new Vector2();

        public List<sprite>         background = new List<sprite>();
        public List<game_object>    static_game_object = new List<game_object>();

        public map_tile(Vector2 position) {
        
            this.position = position;
        }

    }

    public enum world_layer {

        None = 0,
        backgound = 1,      // will only be drawn (no collision)    eg. background image
        world = 2,          // can draw/interact/collide            eg. walls/chest
    }

}
