
namespace Core.world.map {

    using Box2DX.Collision;
    using Box2DX.Common;
    using Box2DX.Dynamics;
    using Core.Controllers.ai;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class Map {

        //public List<Game_Object> allCollidableGameObjects { get; set; } = new List<Game_Object>();
        private List<AI_Controller> all_AI_Controller = new List<AI_Controller>();
        public List<Character> allCharacter { get; set; } = new List<Character>();
        private List<Game_Object> projectils { get; set; } = new List<Game_Object>();

        public readonly World physicsWorld;

        public Map() {
        
            AABB aabb = new ();
            aabb.LowerBound.Set(-100000, -100000);
            aabb.UpperBound.Set(100000, 100000);

            Vec2 gravity = new (0.0f, 0.001f);
            physicsWorld = new World(aabb, gravity, true);
        }

        public virtual void update(float deltaTime) { }

        public bool ray_cast(Vector2 start, Vector2 end, out Vec2 normal, out float distance, out Game_Object? intersected_game_object, bool draw_debug = false, float duration_in_sec = 2.0f) {

            Segment ray = new Segment{
                P1 = new Vec2(start.X, start.Y),
                P2 = new Vec2(end.X, end.Y),
            };

            var shape = physicsWorld.RaycastOne(ray, out distance, out normal, false, null);

            if(shape != null) 
                intersected_game_object = (Game_Object)shape.GetBody().GetUserData();
            else
                intersected_game_object = null;

            if(draw_debug) {

                if(shape == null)
                    Game.Instance.draw_debug_line(start, end, duration_in_sec, DebugColor.Red);
                else {

                    Vector2 direaction = end-start;
                    Vector2 normal_display = new Vector2(normal.X, normal.Y)*10;

                    Game.Instance.draw_debug_line(start, start + (direaction * distance), duration_in_sec, DebugColor.Red);    // start - hit
                    Game.Instance.draw_debug_line(start + (direaction * distance), end, duration_in_sec, DebugColor.Green);    // hit - end
                    Game.Instance.draw_debug_line(start + (direaction * distance), start + (direaction * distance) + normal_display, duration_in_sec, DebugColor.Blue);    // normal
                }


                //duration_in_sec
            }

            return shape != null;
        }

        public int levelWidth { get; set; } = 10000;
        public int levelHeight { get; set; } = 10000;
        public int tileHeight { get; set; }
        public int tileWidth { get; set; }
        public int TilesOnScreenWidth { get; set; }
        public int TilesOnScreenHeight { get; set; }

        public void add_AI_Controller(AI_Controller ai_Controller) { all_AI_Controller.Add(ai_Controller); }

        internal struct Tile_Data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public Tile_Data(int texture_slot, Matrix4 modle_matrix) {
                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        public virtual void Draw_Imgui() {


            foreach(var character in this.allCharacter)
                character.draw_imgui();

            for(int x = 0; x < this.world.Count; x++)
                this.world[x].draw_imgui();
        }

        [Obsolete("")]
        public void Add_Game_Object(Game_Object game_object) { this.world.Add(game_object); }

        public void Remove_Game_Object(Game_Object game_object) {
            this.world.Remove(game_object);
            this.physicsWorld.DestroyBody(game_object.collider.body);
        }
        
        public void Add_Character(AI_Controller ai_controller, Vector2? position = null, float rotation = 0) {
            all_AI_Controller.Add(ai_controller);
            foreach (var character in ai_controller.characters) {
                Add_empty_Character(character, position);
                character.transform.rotation = rotation;
            }
        }

        public void Add_empty_Character(Character character, Vector2? position = null) {
        
            if(position != null)
                character.transform.position = position.Value;

            BodyDef def = new BodyDef();
            def.LinearDamping = 1.0f;
            def.AllowSleep = false;
            if(position != null)
                def.Position.Set(position.Value.X, position.Value.Y);
            else
                def.Position.Set(1, 1);

            float radius = Math.Abs(character.transform.size.X / 2);
            if(character.collider != null)
                radius = Math.Abs((character.transform.size.X / 2) + (character.collider.offset.size.X / 2));

            CircleDef circleDef = new CircleDef();
            circleDef.Radius = radius;
            circleDef.Density = 1f;
            circleDef.Friction = 0.3f;

            Body body = this.physicsWorld.CreateBody(def);
            body.CreateShape(circleDef);
            body.IsDynamic();
            body.SetMassFromShapes();
            body.SetUserData(character);

            if(character.collider != null)
                character.collider.body = body;
            else
                character.Add_Collider(new Collider(body));

            this.allCharacter.Add(character);
            Console.WriteLine($"Adding character [{character}] to map. Current count: {this.allCharacter.Count} ");
        }

        public void Add_Sprite(Sprite sprite) {

            this.backgound.Add(sprite);
        }

        public void Add_Sprite(World_Layer world_Layer, Sprite sprite) {
            if(sprite == null)
                return;

            switch(world_Layer) {

                case World_Layer.None: break;
                case World_Layer.world:
                    // Console.WriteLine($"add_sprite() with argument [World_Layer = World_Layer.world] is not implemented yet");
                    // world.Add(new game_object().set_sprite(sprite));
                    break;

                case World_Layer.backgound:
                    this.backgound.Add(sprite);
                    break;
            }
        }

        public void Set_Background_Image(string image_path) {

            Texture background_texture = Resource_Manager.Get_Texture(image_path, false);
            Vector2 view_size = Game.Instance.camera.Get_View_Size_In_World_Coord();
            Vector2 original_sprite_size = new (background_texture.Width, background_texture.Height);
            Vector2 scale_factor = view_size / original_sprite_size * Game.Instance.camera.GetScale();
            Vector2 sprite_size = original_sprite_size * scale_factor;
            Vector2 sprite_position = Vector2.Zero;
            Transform background_transform = new (sprite_position, sprite_size, 0, Mobility.STATIC);
            Sprite background_sprite = new (background_transform, background_texture);

            this.backgound.Clear();
            this.backgound.Add(background_sprite);
        }

        public void Add_Background_Sprite(Sprite sprite, Vector2 position, bool use_cellSize = true) {

            var current_tile = this.Get_Correct_Map_Tile(position);
            sprite.transform.position = position;
            if(use_cellSize) 
                sprite.transform.size = new Vector2(this.cellSize);

            current_tile.background.Add(sprite);
        }

        [Obsolete("Use [add_static_collider_AAABB()] instead")]
        public void Add_Static_Game_Object(Game_Object new_game_object, Vector2 position, bool use_cellSize = true) {

            var current_tile = this.Get_Correct_Map_Tile(position);

            new_game_object.transform.position = position;
            new_game_object.transform.mobility = Mobility.STATIC;

            if(new_game_object.collider != null) 
                new_game_object.collider.offset.mobility = Mobility.STATIC;

            if(use_cellSize) 
                new_game_object.transform.size = new Vector2(this.cellSize);

            current_tile.staticGameObject.Add(new_game_object);
            //this.allCollidableGameObjects.Add(new_game_object);
        }

        public void add_static_collider_AAABB(Transform transform, bool use_cell_size = true) {

            if(use_cell_size) 
                transform.size = new Vector2(this.cellSize);
            

            BodyDef def = new ();
            def.Position.Set(transform.position.X, transform.position.Y);
            def.AllowSleep = false;
            def.FixedRotation = true;

            PolygonDef polygonDef = new ();
            polygonDef.SetAsBox(transform.size.X / 2, transform.size.Y / 2);
            polygonDef.Density = 1f;

            Body body = this.physicsWorld.CreateBody(def);
            body.CreateShape(polygonDef);
            body.IsStatic();

            var current_tile = this.Get_Correct_Map_Tile(transform.position);
            current_tile.staticColliders.Add(body);
        }

        public void Force_Clear_mapTiles() {

            this.mapTiles.Clear();

            if(Game.Instance.show_performance) 
                DebugData.numOfTiels = 0;
        }

        public Map Generate_Backgound_Tile(int width, int height) {
            // ------------------------ SETUP ------------------------
            Texture texture_atlas = Resource_Manager.Get_Texture("assets/textures/terrain.png", false);

            Random random = new ();
            double missing_time_rate = 0f;

            float offset_x = ((float)width - 1) / 2 * this.cellSize;
            float offset_y = ((float)height - 1) / 2 * this.cellSize;

            // Loop through the tiles and add them to the _positions list
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {

                    if(random.NextDouble() < missing_time_rate)
                        continue;

                    Transform loc_trans_buffer = new (new Vector2((x * this.cellSize) - offset_x, (y * this.cellSize) - offset_y),
                        new Vector2(this.cellSize),
                        0, // (float)utility.Degree_To_Radians(_rotations[random.Next(0, 3)]),
                        Mobility.STATIC);

                    // ============================ GRAS FILD ============================
                    if(random.NextDouble() < 0.01f) 
                        this.backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).Select_Texture_Region(32, 64, 3, 30));
                    else if(random.NextDouble() < 0.03f) 
                        this.backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).Select_Texture_Region(32, 64, 5, 26));
                    else if(random.NextDouble() < 0.1f) 
                        this.backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).Select_Texture_Region(32, 64, 10, 5));
                    else 
                        this.backgound.Add(new Sprite(loc_trans_buffer, texture_atlas).Select_Texture_Region(32, 64, 4, 28));
                }
            }

            return this;
        }

        public void Load_Level(string tmxFilePath, string tsxFilePath, string tilesetImageFilePath) {

            Level_Data levelData = Level_Parser.Parse_Level(tmxFilePath, tsxFilePath);
            Map_Data mapData = levelData.Map;
            this.levelWidth = mapData.LevelPixelWidth;
            this.levelHeight = mapData.LevelPixelHeight;
            this.tileWidth = mapData.TileWidth;
            this.tileHeight = mapData.TileHeight;
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

                        Transform tileTransform = new (
                            new Vector2(x * mapData.TileWidth, y * mapData.TileHeight),
                            new Vector2(mapData.TileWidth, mapData.TileHeight),
                            0,
                            Mobility.STATIC);

                        Sprite tileSprite = new Sprite(tileTransform, tilesetTexture)
                                            .Select_Texture_RegionNew(tilesetColumns, tilesetRows, tileColumn, tileRow, tileGID, textureWidth, textureHeight);

                        if(tilesetData.CollidableTiles.ContainsKey(tileIndex) && tilesetData.CollidableTiles[tileIndex]) {
                            Transform buffer = tileColumn == 0 && tileRow == 5
                                ? new Transform(new Vector2(0, -10), new Vector2(0, -23), 0, Mobility.STATIC)
                                : new Transform(Vector2.Zero, Vector2.Zero, 0, Mobility.STATIC);

                            Game_Object newGameObject = new Game_Object(tileTransform)
                                .Set_Sprite(tileSprite)
                                .Add_Collider(new Collider(Collision_Shape.Square) { blocking = true }.Set_Offset(buffer))
                                .Set_Mobility(Mobility.STATIC);

                            this.Add_Static_Game_Object(newGameObject, tileTransform.position);

                            if(layerIndex == 0) 
                                this.Add_Background_Sprite(tileSprite, tileTransform.position);
                            
                        }
                        else {
                            if(layerIndex == 0) 
                                this.Add_Background_Sprite(tileSprite, tileTransform.position);
                            
                            //else 
                            //    this.allCollidableGameObjects.Add(new Game_Object(tileTransform).Set_Sprite(tileSprite));
                            
                        }
                    }
                }
            }
        }

        // ================================================================= internal =================================================================

        internal void Draw() {

            Vector2 camera_pos = Game.Instance.camera.transform.position;
            Vector2 camera_size = Game.Instance.camera.Get_View_Size_In_World_Coord() + new Vector2(this.cellSize * 2);
            float tiel_size = this.tileSize * this.cellSize;

            // Draw the background first
            for(int x = 0; x < this.backgound.Count; x++)
                this.backgound[x].Draw();

            foreach(var tile in this.mapTiles) {
                float overlapX = (camera_size.X / 2) + (tiel_size / 2) - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = (camera_size.Y / 2) + (tiel_size / 2) - Math.Abs(camera_pos.Y - tile.Key.Y);

                if(overlapX > 0 && overlapY > 0) {

                    if(Game.Instance.show_performance)
                        DebugData.numOfTielsDisplayed++;
                    foreach(var sprite in tile.Value.background) 
                        sprite.Draw();
                }
            }

            foreach(var character in this.allCharacter) 
                character.Draw();

            for(int x = 0; x < this.world.Count; x++) 
                this.world[x].Draw();
        }

        internal void Draw_Debug() {

            Vector2 camera_pos = Game.Instance.camera.transform.position;
            Vector2 camera_size = Game.Instance.camera.Get_View_Size_In_World_Coord() + new Vector2(300);
            float tiel_size = this.tileSize * this.cellSize;

            foreach(var tile in this.mapTiles) {

                float overlapX = (camera_size.X / 2) + (tiel_size / 2) - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = (camera_size.Y / 2) + (tiel_size / 2) - Math.Abs(camera_pos.Y - tile.Key.Y);

                if(overlapX > 0 && overlapY > 0) {
                    foreach(var game_object in tile.Value.staticGameObject) 
                        game_object.Draw_Debug();
                }
            }

            foreach(var character in this.allCharacter) 
                character.Draw_Debug();

            for(int x = 0; x < this.world.Count; x++) 
                this.world[x].Draw_Debug();
        }

        internal void update_internal(float deltaTime) {

            this.physicsWorld.Step(deltaTime * 10, this.velocityIterations, this.positionIterations);

            foreach(var character in this.allCharacter) {
                character.Update_position();
                character.Update(deltaTime);
            }

            foreach (var AI_Controller in all_AI_Controller)
                AI_Controller.Update(deltaTime);

            for (int x = 0; x < this.world.Count; x++) {

                if(world[x].collider != null) {
                    if(world[x].collider.body != null) {

                        world[x].Update_position();
                    }
                }
                world[x].Update(deltaTime);
            }

            update(deltaTime);
        }

        // ========================================== private ==========================================
        private List<Sprite> backgound { get; set; } = new List<Sprite>();       // change to list of lists => to reduce drawcalls
        private List<Game_Object> world { get; set; } = new List<Game_Object>();
        private readonly int velocityIterations = 6;
        private readonly int positionIterations = 1;
        
        // ------------------------------------------ tiles ------------------------------------------
        protected float minDistancForCollision = 1600;
        protected int cellSize = 200;
        protected int tileSize = 8;     // 8 default_sprites fit in one tile

        private Dictionary<Vector2i, Map_Tile> mapTiles { get; set; } = new Dictionary<Vector2i, Map_Tile>();

        private Map_Tile Get_Correct_Map_Tile(Vector2 position) {

            int final_tileSize = this.tileSize * this.cellSize;
            Vector2i key = new ((int)System.Math.Floor(position.X / final_tileSize), (int)System.Math.Floor(position.Y / final_tileSize));
            key *= final_tileSize;
            key += new Vector2i(final_tileSize / 2, final_tileSize / 3);

            if(!this.mapTiles.ContainsKey(key)) {

                this.mapTiles.Add(key, new Map_Tile(key));

                if(Game.Instance.show_performance)
                    DebugData.numOfTiels++;
            }

            this.mapTiles.TryGetValue(key, out Map_Tile current_tile);
            return current_tile;
        }
    }

    public struct Map_Tile {

        public Vector2 position = new ();

        public List<Sprite> background = new ();
        public List<Game_Object> staticGameObject = new ();
        public List<Body> staticColliders = new ();

        public Map_Tile(Vector2 position) {
            this.position = position;
        }
    }

    public enum World_Layer {
        None = 0,
        backgound = 1,      // will only be drawn (no collision)    eg. background image
        world = 2,          // can Draw/interact/collide            eg. walls/chest
    }
}
