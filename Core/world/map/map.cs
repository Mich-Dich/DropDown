
namespace Core.world.map {

    using Box2DX.Collision;
    using Box2DX.Common;
    using Box2DX.Dynamics;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;

    public class Map {

        public List<Game_Object>    all_collidable_game_objects { get; set; } = new List<Game_Object>();
        private List<Character>     _all_character{ get; set; } = new List<Character>();

        private World physics_world;
        public Map() {

            AABB aabb = new AABB();
            aabb.LowerBound.Set(-100000, -100000);
            aabb.UpperBound.Set(100000, 100000);

            Vec2 gravity = new Vec2(0.0f, 0.001f);
            physics_world = new World(aabb, gravity, true);
        }

        public int levelWidth { get; set; } = 10000;
        public int levelHeight { get; set; } = 10000;
        public int tileHeight { get; set; }
        public int tileWidth { get; set; }
        public int TilesOnScreenWidth { get; set; }
        public int TilesOnScreenHeight { get; set; }

        /// <summary>
        /// Represents data for a tile, including texture slot and model matrix.
        /// </summary>
        internal struct tile_data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public tile_data(int texture_slot, Matrix4 modle_matrix) {

                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        /// <summary>
        /// Draws ImGui elements specific to the map.
        /// </summary>
        public virtual void Draw_Imgui() { }

        /// <summary>
        /// Adds a game object to the map's world.
        /// </summary>
        public void Add_Game_Object(Game_Object game_object) {

            world.Add(game_object);
            all_collidable_game_objects.Add(game_object);
            //Console.WriteLine($"Adding game_object [{game_object}] to world. Current count: {world.Count} ");
        }

        /// <summary>
        /// Adds a character to the map's world.
        /// </summary>
        public void Add_Character(Character character, Vector2? position = null) {

            if(position != null)
                character.transform.position = position.Value;

            BodyDef def = new BodyDef();
            def.Position.Set(1,1);
            def.LinearDamping = 1.0f;
            def.AllowSleep = false;

            float radius = Math.Abs(character.transform.size.X/2);
            if(character.collider != null)
                radius = Math.Abs(character.transform.size.X/2 + character.collider.offset.size.X/2);

            CircleDef circleDef = new CircleDef{ Radius = radius };
            circleDef.Density = 1f;
            circleDef.Friction = 0.3f;

            Body body = physics_world.CreateBody(def);
            body.CreateShape(circleDef);
            body.IsDynamic();
            body.SetMassFromShapes();

            if(character.collider != null)
                character.collider.body = body;
            else
                character.Add_Collider(new Collider(body));


            _all_character.Add(character);
            //Console.WriteLine($"added body to character. Current count: [{physics_world.GetBodyCount()}]");
            //Console.WriteLine($"Adding character [{character}] to map. Current count: {_all_character.Count} ");
        }

        /// <summary>
        /// Adds a background sprite to the map's background layer.
        /// </summary>
        public void Add_Sprite(Sprite sprite) { backgound.Add(sprite); }

        /// <summary>
        /// Adds a sprite to the specified world layer.
        /// </summary>
        /// <param name="world_layer">The world layer to add the sprite to.</param>
        public void Add_Sprite(world_layer world_layer, Sprite sprite) {

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

        /// <summary>
        /// Sets the background image of the map.
        /// </summary>
        /// <param name="image_path">The path to the background image.</param>
        public void Set_Background_Image(string image_path) {

            Texture background_texture = Resource_Manager.Get_Texture(image_path, false);
            Vector2 view_size = Game.instance.camera.Get_View_Size_In_World_Coord();
            Vector2 original_sprite_size = new Vector2(background_texture.Width, background_texture.Height);
            Vector2 scale_factor = view_size / original_sprite_size;
            Vector2 sprite_size = original_sprite_size * scale_factor;
            Vector2 sprite_position = Vector2.Zero;
            Transform background_transform = new Transform(sprite_position, sprite_size, 0, Mobility.STATIC);
            Sprite background_sprite = new Sprite(background_transform, background_texture);

            backgound.Clear();
            backgound.Add(background_sprite);
        }

        // ================================================================= internal =================================================================

        /// <summary>
        /// Draws the map, including background sprites and world objects.
        /// </summary>
        internal void Draw() {

            Vector2 camera_pos = Game.instance.camera.transform.position;
            Vector2 camera_size = Game.instance.camera.Get_View_Size_In_World_Coord() + new Vector2(cell_size * 2);
            float tiel_size = tile_size * cell_size;

            foreach(var tile in map_tiles) {

                float overlapX = camera_size.X / 2 + tiel_size / 2 - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = camera_size.Y / 2 + tiel_size / 2 - Math.Abs(camera_pos.Y - tile.Key.Y);

                if(overlapX > 0 && overlapY > 0) {

                    debug_data.num_of_tiels_displayed++;
                    foreach(var sprite in tile.Value.background) {
                        sprite.Draw();
                    }
                }
            }

            foreach(var character in _all_character)
                character.Draw();

            for(int x = 0; x < backgound.Count; x++)
                backgound[x].Draw();

            for(int x = 0; x < world.Count; x++)
                world[x].Draw();
        }

        /// <summary>
        /// Draws debug information for the map, such as collision shapes of game objects.
        /// </summary>
        internal void Draw_Debug() {

            Vector2 camera_pos = Game.instance.camera.transform.position;
            Vector2 camera_size = Game.instance.camera.Get_View_Size_In_World_Coord() + new Vector2(300);
            float tiel_size = tile_size * cell_size;

            foreach(var tile in map_tiles) {

                float overlapX = camera_size.X / 2 + tiel_size / 2 - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = camera_size.Y / 2 + tiel_size / 2 - Math.Abs(camera_pos.Y - tile.Key.Y);

                if(overlapX > 0 && overlapY > 0) {

                    foreach(var game_object in tile.Value.static_game_object) {
                        game_object.Draw_Debug();
                    }
                }
            }

            foreach(var character in _all_character)
                character.Draw_Debug();

            for(int x = 0; x < world.Count; x++)
                world[x].Draw_Debug();
        }

        /// <summary>
        /// Updates the map's logic and physics.
        /// </summary>
        /// <param name="delta_time">The time elapsed since the last update.</param>
        private int velocityIterations = 6;
        private int positionIterations = 1;
        internal void Update(float delta_time) {

            physics_world.Step(delta_time*10, velocityIterations, positionIterations);

            foreach (var character in _all_character) {
                character.update_position();
                character.Update(delta_time);
            }

            for(int x = 0; x < this.all_collidable_game_objects.Count; x++)
                this.all_collidable_game_objects[x].Update(Game_Time.delta);

        }

        /// <summary>
        /// Adds a background sprite to the map tile at the specified position.
        /// </summary>
        /// <param name="sprite">The sprite to add to the background.</param>
        /// <param name="position">The position where the sprite should be added.</param>
        /// <param name="use_cell_size">Flag indicating whether to Use the cell size for the sprite.</param>
        public void Add_Background_Sprite(Sprite sprite, Vector2 position, bool use_cell_size = true) {

            var current_tile = Get_Correct_Map_Tile(position);

            sprite.transform.position = position;
            if(use_cell_size)
                sprite.transform.size = new Vector2(cell_size);

            current_tile.background.Add(sprite);
        }

        /// <summary>
        /// Adds a static game object to the map tile at the specified position.
        /// </summary>
        /// <param name="new_game_object">The new game object to add to the map.</param>
        /// <param name="position">The position where the game object should be added.</param>
        /// <param name="use_cell_size">Flag indicating whether to Use the cell size for the game object.</param>
        public void Add_Static_Game_Object(Game_Object new_game_object, Vector2 position, bool use_cell_size = true) {

            var current_tile = Get_Correct_Map_Tile(position);

            new_game_object.transform.position = position;
            new_game_object.transform.mobility = Mobility.STATIC;
            if(new_game_object.collider != null)
                new_game_object.collider.offset.mobility = Mobility.STATIC;
            if(use_cell_size)
                new_game_object.transform.size = new Vector2(cell_size);

            current_tile.static_game_object.Add(new_game_object);
            all_collidable_game_objects.Add(new_game_object);
        }

        public void add_static_collider_AAABB(Transform transform, bool use_cell_size = true) {

            if(use_cell_size)
                transform.size = new Vector2(cell_size);

            BodyDef def = new BodyDef();
            def.Position.Set(transform.position.X, transform.position.Y);
            def.AllowSleep = false;
            def.FixedRotation = true;

            PolygonDef polygonDef = new PolygonDef();
            polygonDef.SetAsBox(transform.size.X / 2, transform.size.Y / 2);
            polygonDef.Density = 1f;
            
            Body body = physics_world.CreateBody(def);
            body.CreateShape(polygonDef);
            body.IsStatic();
            
            var current_tile = Get_Correct_Map_Tile(transform.position);
            current_tile.static_colliders.Add(body);

        }

        /// <summary>
        /// Registers a new player character.
        /// </summary>
        /// <param name="character">The character to register as a player.</param>
        public void Register_Player_NEW(Character character) { }

        /// <summary>
        /// Clears all map tiles and associated debug data.
        /// </summary>
        public void Force_Clear_Map_Tiles() {
            map_tiles.Clear();
            debug_data.num_of_tiels = 0;
        }


        public Map Generate_Backgound_Tile(int width, int height) {

            // ------------------------ SETUP ------------------------
            Texture texture_atlas = Resource_Manager.Get_Texture("assets/textures/terrain.png", false);

            Random random = new Random();
            double missing_time_rate = 0f;

            float offset_x = ((float)width - 1) / 2 * cell_size;
            float offset_y = ((float)height - 1) / 2 * cell_size;

            // Loop through the tiles and add them to the _positions list
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {

                    if(random.NextDouble() < missing_time_rate)    // Skip adding tiles at certain positions (e.g., missing tiles)
                        continue;

                    Transform loc_trans_buffer = new Transform(new Vector2(x * cell_size - offset_x, y * cell_size - offset_y),
                        new Vector2(cell_size),
                        0, //(float)utility.degree_to_radians(_rotations[random.Next(0, 3)]),
                        Mobility.STATIC);

                    // ============================ GRAS FILD ============================ 
                    if(random.NextDouble() < 0.01f)
                        backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 3, 30));
                    else if(random.NextDouble() < 0.03f)
                        backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 5, 26));
                    else if(random.NextDouble() < 0.1f)
                        backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 10, 5));
                    else
                        backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).select_texture_region(32, 64, 4, 28));
                }
            }

            return this;
        }

        public void Load_Level(string tmxFilePath, string tsxFilePath, string tilesetImageFilePath) {

            Level_Data levelData = Level_Parser.Parse_Level(tmxFilePath, tsxFilePath);
            Map_Data mapData = levelData.Map;
            levelWidth = mapData.LevelPixelWidth;
            levelHeight = mapData.LevelPixelHeight;
            tileWidth = mapData.TileWidth;
            tileHeight = mapData.TileHeight;
            Tileset_Data tilesetData = levelData.Tileset;
            Texture tilesetTexture = Resource_Manager.Get_Texture(tilesetImageFilePath, false);

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

                        Transform tileTransform = new Transform(
                            new Vector2(x * mapData.TileWidth, y * mapData.TileHeight),
                            new Vector2(mapData.TileWidth, mapData.TileHeight),
                            0,
                            Mobility.STATIC);

                        Sprite tileSprite = new Sprite(tileTransform, tilesetTexture)
                                            .select_texture_regionNew(tilesetColumns, tilesetRows, tileColumn, tileRow, tileGID, textureWidth, textureHeight);

                        if(tilesetData.CollidableTiles.ContainsKey(tileIndex) && tilesetData.CollidableTiles[tileIndex]) {
                            Transform buffer = tileColumn == 0 && tileRow == 5
                                ? new Transform(new Vector2(0, -10), new Vector2(0, -23), 0, Mobility.STATIC)
                                : new Transform(Vector2.Zero, Vector2.Zero, 0, Mobility.STATIC);

                            Game_Object newGameObject = new Game_Object(tileTransform)
                                .Set_Sprite(tileSprite)
                                .Add_Collider(new Collider(collision_shape.Square) { Blocking = true }.set_offset(buffer))
                                .Set_Mobility(Mobility.STATIC);

                            Add_Static_Game_Object(newGameObject, tileTransform.position);

                            if(layerIndex == 0)
                                Add_Background_Sprite(tileSprite, tileTransform.position);

                        }
                        else {
                            if(layerIndex == 0)
                                Add_Background_Sprite(tileSprite, tileTransform.position);
                            else
                                all_collidable_game_objects.Add(new Game_Object(tileTransform).Set_Sprite(tileSprite));
                        }
                    }
                }
            }
        }


        // ========================================== private ==========================================

        private List<Sprite> backgound { get; set; } = new List<Sprite>();       // change to list of lists => to reduce drawcalls
        private List<Game_Object> world { get; set; } = new List<Game_Object>();

        // ------------------------------------------ tiles ------------------------------------------
        protected float min_distanc_for_collision = 1600;
        protected int cell_size = 200;
        protected int tile_size = 8;     // 8 default_sprites fit in one tile
        private Dictionary<Vector2i, map_tile> map_tiles { get; set; } = new Dictionary<Vector2i, map_tile>();


        private map_tile Get_Correct_Map_Tile(Vector2 position) {

            int final_tile_size = tile_size * cell_size;
            Vector2i key = new Vector2i((int)System.Math.Floor(position.X / final_tile_size), (int)System.Math.Floor(position.Y / final_tile_size));
            key *= final_tile_size;
            key += new Vector2i(final_tile_size / 2, final_tile_size / 3);

            if(!map_tiles.ContainsKey(key)) {

                map_tiles.Add(key, new map_tile(key));
                debug_data.num_of_tiels++;
            }

            map_tiles.TryGetValue(key, out map_tile current_tile);
            return current_tile;
        }
    }

    public struct map_tile {

        public Vector2 position = new Vector2();

        public List<Sprite> background = new List<Sprite>();
        public List<Game_Object> static_game_object = new List<Game_Object>();
        public List<Body>   static_colliders = new List<Body>();


        public map_tile(Vector2 position) {

            this.position = position;
        }

    }

    public enum world_layer {

        None = 0,
        backgound = 1,      // will only be drawn (no collision)    eg. background image
        world = 2,          // can Draw/interact/collide            eg. walls/chest
    }

}
