namespace Core.world
{
    using Box2DX.Collision;
    using Box2DX.Common;
    using Box2DX.Dynamics;
    using Core.controllers;
    using Core.Controllers.ai;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Core.defaults;
    using Core.Particles;

    public class Map {

        public List<Game_Object> all_game_objects { get; set; } = new List<Game_Object>();
        private readonly List<I_Controller> all_AI_Controller = new();
        public List<Character> allCharacter { get; set; } = new List<Character>();
        public List<Game_Object> projectiles { get; set; } = new List<Game_Object>();
        public List<PowerUp> allPowerUps { get; set; } = new List<PowerUp>();    
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        public bool player_is_spawned { get; private set; } = false;
        public int scoreGoal { get; set; }
        public int previousScoreGoal { get; set; }
        public float ScoreRatio
        {
            get
            {
                return (float)(Core.Game.Instance.Score - previousScoreGoal) / (scoreGoal - previousScoreGoal);
            }
            set { }
        }
        protected bool use_garbage_collector = false;

        public readonly World physicsWorld;
        private const int MaxPhysicsBodies = 400;

        public Map() {

            AABB aabb = new();
            aabb.LowerBound.Set(-100000, -100000);
            aabb.UpperBound.Set(100000, 100000);

            Vec2 gravity = new(0.0f, 0.001f);
            physicsWorld = new World(aabb, gravity, true);
            physicsWorld.SetContactListener(new CollisionListener());

            this.particleSystem = new ParticleSystem(Game.Instance.particleShader);
        }

        public virtual void update(float deltaTime) { }

        public bool ray_cast(Vector2 start, Vector2 end, out Vec2 normal, out float distance, out Game_Object? intersected_game_object, bool draw_debug = false, float duration_in_sec = 2.0f) {

            Segment ray = new() {
                P1 = new Vec2(start.X, start.Y),
                P2 = new Vec2(end.X, end.Y),
            };

            var shape = physicsWorld.RaycastOne(ray, out distance, out normal, false, null);
            intersected_game_object = shape != null ? (Game_Object)shape.GetBody().GetUserData() : null;

            if (draw_debug) {

                if (shape == null)
                    Game.Instance.draw_debug_line(start, end, duration_in_sec, DebugColor.Red);

                else {

                    Vector2 direaction = end - start;
                    Vector2 normal_display = new Vector2(normal.X, normal.Y) * 10;

                    Game.Instance.draw_debug_line(start, start + direaction * distance, duration_in_sec, DebugColor.Red);    // start - hit
                    Game.Instance.draw_debug_line(start + direaction * distance, end, duration_in_sec, DebugColor.Green);    // hit - end
                    Game.Instance.draw_debug_line(start + direaction * distance, start + direaction * distance + normal_display, duration_in_sec, DebugColor.Blue);    // normal
                }
            }

            return shape != null;
        }

        public int levelWidth { get; set; } = 10000;
        public int levelHeight { get; set; } = 10000;
        public int tileHeight { get; set; }
        public int tileWidth { get; set; }
        public int TilesOnScreenWidth { get; set; }
        public int TilesOnScreenHeight { get; set; }

        public virtual void PlayerLevelUp() { }

        internal struct Tile_Data {

            public int texture_slot { get; set; }
            public Matrix4 modle_matrix { get; set; }

            public Tile_Data(int texture_slot, Matrix4 modle_matrix){

                this.texture_slot = texture_slot;
                this.modle_matrix = modle_matrix;
            }
        }

        public virtual void Draw_Imgui() {

            foreach (var character in allCharacter)
                character.draw_imgui();

            for (int x = 0; x < world.Count; x++)
                world[x].draw_imgui();
        }

        [Obsolete("")]
        public void Add_Game_Object(Game_Object game_object) {

            world.Add(game_object);

            if (game_object.transform.mobility == Mobility.DYNAMIC)
                DebugData.colidableObjectsDynamic++;
            else
                DebugData.colidableObjectsStatic++;
        }

        public void Remove_Game_Object(Game_Object game_object) {

            game_object.IsRemoved = true;
            world.Remove(game_object);
            all_game_objects.Remove(game_object);
            projectiles.Remove(game_object);
            allCharacter.Remove(game_object as Character);

            if (game_object.collider != null && game_object.collider.body != null) {

                //int bodyCountBefore = physicsWorld.GetBodyCount();
                game_object.collider.body.SetUserData(null);
                physicsWorld.DestroyBody(game_object.collider.body);
                game_object.collider.body = null;
                //int bodyCountAfter = physicsWorld.GetBodyCount();
            }

            if (game_object.transform.mobility == Mobility.DYNAMIC)
                DebugData.colidableObjectsDynamic--;
            else
                DebugData.colidableObjectsStatic--;
        }

        public void add_AI_Controller(AI_Controller ai_Controller) { all_AI_Controller.Add(ai_Controller); }

        public Character Add_Player(Character character, Vector2? position = null, float rotation = 0.0f, bool IsSensor = false)
        {

            if (player_is_spawned)
                throw new Exception("player already spawned in this map");

            player_is_spawned = true;
            return Add_Character(character, position, rotation, IsSensor);
        }


        public Character Add_Character(Character character, Vector2? position = null, float rotation = 0.0f, bool IsSensor = false)
        {

            if (position != null)
                character.transform.position = position.Value;

            BodyDef def = new();
            def.LinearDamping = 1.0f;
            def.AllowSleep = false;
            def.Angle = rotation;
            if (position != null)
                def.Position.Set(position.Value.X, position.Value.Y);
            else
                def.Position.Set(0, 0);

            float radius = Math.Abs(character.transform.size.X / 2);
            if (character.collider != null)
                radius = Math.Abs(character.transform.size.X / 2 + character.collider.offset.size.X / 2);

            CircleDef circleDef = new();
            circleDef.Radius = radius;
            circleDef.Density = 1f;
            circleDef.Friction = 0.3f;
            circleDef.IsSensor = IsSensor;

            Body body = physicsWorld.CreateBody(def);
            body.CreateShape(circleDef);
            body.IsDynamic();
            body.SetMassFromShapes();
            body.SetUserData(character);

            character.transform.mobility = Mobility.DYNAMIC;
            if (character.collider != null)
            {
                character.collider.body = body;
            }
            else
            {
                Collider newCollider = new(body);
                character.Add_Collider(newCollider);
            }

            allCharacter.Add(character);

            DebugData.colidableObjectsDynamic++;
            return character;
        }

        public Game_Object add_game_object(Game_Object game_object, Vector2 position, Vector2 size)
        {
            game_object.transform.position = position;
            game_object.transform.size = size;

            game_object.collider.body = CreatePhysicsBody(position, size, false);
            game_object.collider.body.SetUserData(game_object);

            return game_object;
        }

        public Body CreatePhysicsBody(Vector2 position, Vector2 size, bool isSensor)
        {
            BodyDef def = new();
            def.Position.Set(position.X, position.Y);
            def.AllowSleep = false;
            def.LinearDamping = 0f;

            PolygonDef polygonDef = new();
            polygonDef.SetAsBox(size.X / 2, size.Y / 2);
            polygonDef.Density = 1f;
            polygonDef.Friction = 0.3f;
            polygonDef.IsSensor = isSensor;

            Body body = physicsWorld.CreateBody(def);
            body.CreateShape(polygonDef);
            body.SetMassFromShapes();

            return body;
        }

        public void Add_Sprite(Sprite sprite)
        {

            backgound.Add(sprite);
        }

        public void Add_Sprite(World_Layer world_Layer, Sprite sprite)
        {

            if (sprite == null)
                return;

            switch (world_Layer)
            {
                case World_Layer.None: break;
                case World_Layer.world:
                    break;

                case World_Layer.backgound:
                    backgound.Add(sprite);
                    break;
            }
        }

        public void Set_Background_Image(string image_path, float scaleMultiplier = 1.0f)
        {

            Texture background_texture = Resource_Manager.Get_Texture(image_path, false);
            Vector2 window_size = Game.Instance.window.Size;
            Vector2 original_sprite_size = new(background_texture.Width, background_texture.Height);
            Vector2 scale_factor = window_size / original_sprite_size * scaleMultiplier;
            Vector2 sprite_size = original_sprite_size * scale_factor;
            Vector2 sprite_position = Vector2.Zero;
            Transform background_transform = new(sprite_position, sprite_size, 0, Mobility.STATIC);
            Sprite background_sprite = new(background_transform, background_texture);

            backgound.Clear();
            backgound.Add(background_sprite);
        }

        public void Add_Background_Sprite(Sprite sprite, Vector2 position, bool use_cellSize = true)
        {

            var current_tile = Get_Correct_Map_Tile(position);
            sprite.transform.position = position;
            if (use_cellSize)
                sprite.transform.size = new Vector2(cellSize);

            current_tile.background.Add(sprite);
        }

        [Obsolete("Use [add_static_collider_AAABB()] instead")]
        public void Add_Static_Game_Object(Game_Object new_game_object, Transform collider_transform_offset, Vector2 position, bool use_cellSize = true, bool use_circle = true, bool is_sensor = false)
        {

            if (use_cellSize)
                new_game_object.transform.size = new Vector2(cellSize);

            BodyDef def = new();
            def.Position.Set(position.X + collider_transform_offset.position.X, position.Y + collider_transform_offset.position.Y);
            def.AllowSleep = false;
            def.FixedRotation = true;

            Body body = physicsWorld.CreateBody(def);
            body.IsStatic();
            body.SetUserData(new_game_object);

            if (use_circle)
            {

                CircleDef circleDef = new();
                circleDef.IsSensor = is_sensor;
                circleDef.Radius = (new_game_object.transform.size.X / 2) + (collider_transform_offset.size.X / 2);
                circleDef.Density = 1f;

                body.CreateShape(circleDef);

            }
            else
            {

                PolygonDef polygonDef = new();
                polygonDef.SetAsBox((new_game_object.transform.size.X / 2) + (collider_transform_offset.size.X / 2),
                    (new_game_object.transform.size.Y / 2) + (collider_transform_offset.size.X / 2));
                polygonDef.Density = 1f;

                body.CreateShape(polygonDef);
            }

            if (new_game_object.collider != null)
                new_game_object.collider.body = body;

            else
            {
                Collider newCollider = new(body);
                new_game_object.Add_Collider(newCollider);
            }


            if (use_circle)
                new_game_object.collider.shape = Collision_Shape.Circle;
            else
                new_game_object.collider.shape = Collision_Shape.Square;

            new_game_object.collider.offset = collider_transform_offset;
            new_game_object.collider.offset.mobility = Mobility.STATIC;
            new_game_object.transform.position = position;
            new_game_object.transform.mobility = Mobility.STATIC;

            var current_tile = Get_Correct_Map_Tile(position);
            current_tile.staticGameObject.Add(new_game_object);

            DebugData.colidableObjectsStatic++;
        }

        public void add_static_collider_AAABB(Transform transform, bool use_cell_size = true)
        {

            if (use_cell_size)
                transform.size = new Vector2(cellSize);

            BodyDef def = new();
            def.Position.Set(transform.position.X, transform.position.Y);
            def.AllowSleep = false;
            def.FixedRotation = true;

            PolygonDef polygonDef = new();
            polygonDef.SetAsBox(transform.size.X / 2, transform.size.Y / 2);
            polygonDef.Density = 1f;

            Body body = physicsWorld.CreateBody(def);
            body.CreateShape(polygonDef);
            body.IsStatic();

            var current_tile = Get_Correct_Map_Tile(transform.position);
            current_tile.staticColliders.Add(body);

            DebugData.colidableObjectsStatic++;
        }

        public void add_static_collider_Circle(Transform transform, bool use_cell_size = true, bool is_sensor = false)
        {

            if (use_cell_size)
                transform.size = new Vector2(cellSize);

            BodyDef def = new();
            def.Position.Set(transform.position.X, transform.position.Y);
            def.AllowSleep = false;
            def.FixedRotation = true;

            CircleDef circleDef = new();
            circleDef.IsSensor = is_sensor;
            circleDef.Radius = transform.size.X / 2;
            circleDef.Density = 1f;

            Body body = physicsWorld.CreateBody(def);
            body.CreateShape(circleDef);
            body.IsStatic();

            var current_tile = Get_Correct_Map_Tile(transform.position);
            current_tile.staticColliders.Add(body);

            DebugData.colidableObjectsStatic++;
        }

        public void AddParticleSystem(ParticleSystem particleSystem)
        {
            particleSystems.Add(particleSystem);
        }

        public void RemoveParticleSystem(ParticleSystem particleSystem)
        {
            particleSystems.Remove(particleSystem);
        }

        public void Force_Clear_mapTiles()
        {

            mapTiles.Clear();

            if (Game.Instance.show_performance)
                DebugData.numOfTiels = 0;
        }

        public Map Generate_Backgound_Tile(int width, int height)
        {
            // ------------------------ SETUP ------------------------
            Texture texture_atlas = Resource_Manager.Get_Texture("assets/textures/terrain.png", false);

            Random random = new();
            double missing_time_rate = 0f;

            float offset_x = ((float)width - 1) / 2 * cellSize;
            float offset_y = ((float)height - 1) / 2 * cellSize;

            // Loop through the tiles and add them to the _positions list
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if (random.NextDouble() < missing_time_rate)
                        continue;

                    var position = new Vector2(x * cellSize - offset_x, y * cellSize - offset_y);

                    // ============================ GRAS FILD ============================
                    if (random.NextDouble() < 0.01f)
                        Add_Background_Sprite(new Sprite(texture_atlas).Select_Texture_Region(32, 64, 0, 12),
                            position);
                    else if (random.NextDouble() < 0.03f)
                        Add_Background_Sprite(new Sprite(texture_atlas).Select_Texture_Region(32, 64, 1, 12),
                            position);
                    else if (random.NextDouble() < 0.1f)
                        Add_Background_Sprite(new Sprite(texture_atlas).Select_Texture_Region(32, 64, 2, 12),
                            position);
                    else
                        Add_Background_Sprite(new Sprite(texture_atlas).Select_Texture_Region(32, 64, 1, 10),
                            position);
                }
            }

            return this;
        }

        public void Load_Level(string tmxFilePath, string tsxFilePath, string tilesetImageFilePath)
        {

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

            for (int layerIndex = 0; layerIndex < mapData.Layers.Count; layerIndex++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    for (int x = 0; x < mapData.Width; x++)
                    {

                        int tileGID = mapData.Layers[layerIndex].Tiles[x, y];
                        if (tileGID <= 0)
                            continue;


                        int tileIndex = tileGID - tilesetData.FirstGid;
                        if (tileIndex < 0)
                            continue;


                        int tileRow = tileIndex / tilesetColumns;
                        int tileColumn = tileIndex % tilesetColumns;

                        Transform tileTransform = new(
                            new Vector2(x * mapData.TileWidth, y * mapData.TileHeight),
                            new Vector2(mapData.TileWidth, mapData.TileHeight),
                            0,
                            Mobility.STATIC);

                        Sprite tileSprite = new Sprite(tileTransform, tilesetTexture)
                                            .Select_Texture_RegionNew(tilesetColumns, tilesetRows, tileColumn, tileRow, tileGID, textureWidth, textureHeight);

                        if (tilesetData.CollidableTiles.ContainsKey(tileIndex) && tilesetData.CollidableTiles[tileIndex])
                        {
                            Transform buffer = tileColumn == 0 && tileRow == 5
                                ? new Transform(new Vector2(0, -10), new Vector2(0, -23), 0, Mobility.STATIC)
                                : new Transform(Vector2.Zero, Vector2.Zero, 0, Mobility.STATIC);

                            Game_Object newGameObject = new Game_Object(tileTransform)
                                .Set_Sprite(tileSprite)
                                .Add_Collider(new Collider(Collision_Shape.Square) { blocking = true }.Set_Offset(buffer))
                                .Set_Mobility(Mobility.STATIC);

                            Add_Static_Game_Object(newGameObject, new Transform(), tileTransform.position);

                            if (layerIndex == 0)
                                Add_Background_Sprite(tileSprite, tileTransform.position);

                        }
                        else
                        {
                            if (layerIndex == 0)
                                Add_Background_Sprite(tileSprite, tileTransform.position);

                            //else 
                            //    this.allCollidableGameObjects.Add(new Game_Object(tileTransform).Set_Sprite(tileSprite));

                        }
                    }
                }
            }
        }

        private void ResetPhysicsWorld() {

            Body body = physicsWorld.GetBodyList();
            while (body != null) {

                Body nextBody = body.GetNext();
                var userData = body.GetUserData();
                if (!(userData is Character)) {

                    body.SetUserData(null);
                    physicsWorld.DestroyBody(body);
                }

                body = nextBody;
            }

            world.Clear();
            projectiles.Clear();

            DebugData.colidableObjectsStatic = 0;
            DebugData.colidableObjectsDynamic = 0;
        }

        // Particle System Testing

        public void AddEmitter(Emitter emitter)
        {
            this.particleSystem.AddEmitter(emitter);
        }

        public void AddParticles(IEnumerable<Particle> particles)
        {
            this.particleSystem.AddParticles(particles);
            Console.WriteLine($"Added {particles.Count()} particles");
        }

        public void AddForceField(IForceField forceField)
        {
            this.particleSystem.AddForceField(forceField);
        }


        // ================================================================= internal =================================================================

        internal void Draw()
        {
            Vector2 camera_pos = Game.Instance.camera.transform.position;
            Vector2 camera_size = Game.Instance.camera.Get_View_Size_In_World_Coord() + new Vector2(cellSize * 2);
            float tile_size = tileSize * cellSize;

            // Draw tiles within the camera view
            foreach (var tile in mapTiles)
            {
                float overlapX = camera_size.X / 2 + tile_size / 2 - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = camera_size.Y / 2 + tile_size / 2 - Math.Abs(camera_pos.Y - tile.Key.Y);

                if (overlapX > 0 && overlapY > 0)
                {
                    if (Game.Instance.show_performance)
                        DebugData.numOfTielsDisplayed++;

                    foreach (var sprite in tile.Value.background)
                        sprite.Draw();
                    foreach (var game_object in tile.Value.staticGameObject)
                        game_object.Draw();
                }
            }

            // Draw the background sprites
            for (int x = 0; x < backgound.Count; x++)
                backgound[x].Draw();

            // Draw particle systems
            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Render();
            }

            // Draw the main particle system (if you have one)
            this.particleSystem.Render();

            // Rebind the default sprite shader
            Game.Instance.defaultSpriteShader.Use();
            Game.Instance.defaultSpriteShader.Set_Matrix_4x4("projection", Game.Instance.camera.Get_Projection_Matrix());

            // Draw all characters
            foreach (var character in allCharacter)
                if (!character.IsRemoved)
                    character.Draw();

            // Draw other game objects
            for (int x = 0; x < world.Count; x++)
                if (!world[x].IsRemoved)
                    world[x].Draw();
        }


        internal void Draw_Debug() {

            Vector2 camera_pos = Game.Instance.camera.transform.position;
            Vector2 camera_size = Game.Instance.camera.Get_View_Size_In_World_Coord() + new Vector2(300);
            float tiel_size = tileSize * cellSize;

            foreach (var tile in mapTiles) {

                float overlapX = camera_size.X / 2 + tiel_size / 2 - Math.Abs(camera_pos.X - tile.Key.X);
                float overlapY = camera_size.Y / 2 + tiel_size / 2 - Math.Abs(camera_pos.Y - tile.Key.Y);

                if (overlapX > 0 && overlapY > 0) {

                    foreach (var game_object in tile.Value.staticGameObject)
                        game_object.Draw_Debug();
                }
            }

            foreach (var character in allCharacter)
                if (!character.IsRemoved)
                    character.Draw_Debug();

            for (int x = 0; x < world.Count; x++)
                if (!world[x].IsRemoved)
                    world[x].Draw_Debug();
        }

        internal void update_internal(float deltaTime) {

            if(Game.Instance.play_state == Play_State.LevelUp) { return; }
            if(Game.Instance.play_state == Play_State.InGameMenu) { return; }
            if(Game.Instance.play_state == Play_State.PauseMenuSkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PauseAbilitySkillTree) { return; }
            if(Game.Instance.play_state == Play_State.PausePowerupSkillTree) { return; }


            physicsWorld.Step(deltaTime * 10, velocityIterations, positionIterations);

            List<Character> charactersToRemove = new();

            foreach (var character in allCharacter) {

                character.Update_position();
                character.Update(deltaTime);
                if (character.health <= 0 && character.auto_remove_on_death)
                    charactersToRemove.Add(character);
            }

            foreach (var AI_Controller in all_AI_Controller)
                AI_Controller.Update(deltaTime);

            foreach (var game_object in all_game_objects.ToList()) {

                if (!game_object.IsRemoved)
                    game_object.Update(deltaTime);
            }

            for (int x = 0; x < world.Count; x++) {

                if(world[x].IsRemoved)
                    continue;

                if (world[x].collider != null) {
                    if (world[x].collider.body != null)
                        world[x].Update_position();
                }
                world[x].Update(deltaTime);
            }

            foreach (var character in charactersToRemove) {

                allCharacter.Remove(character);
                if (character.death_callback != null)
                    character.death_callback();
            }

            update(deltaTime);

            Game.Instance.camera.transform.Update(); // Update the camera Shake logic...

            // Update the particle system
            this.particleSystem.Update(deltaTime, Game.Instance.player.transform.position);

            if (use_garbage_collector) {

                if (physicsWorld.GetBodyCount() > MaxPhysicsBodies) {

                    Console.WriteLine($"WARNING: Physics body count exceeded limit ({MaxPhysicsBodies}). Resetting physics world.");
                    ResetPhysicsWorld();
                }
            }

        }

        // ========================================== private ==========================================
        private List<Sprite> backgound { get; set; } = new List<Sprite>();       // change to list of lists => to reduce drawcalls
        private List<Game_Object> world { get; set; } = new List<Game_Object>();
        private readonly int velocityIterations = 6;
        private readonly int positionIterations = 1;
        private ParticleSystem particleSystem;
        // ------------------------------------------ tiles ------------------------------------------
        protected float minDistancForCollision = 1600;
        protected int cellSize = 200;
        protected int tileSize = 8;     // 8 default_sprites fit in one tile

        private Dictionary<Vector2i, Map_Tile> mapTiles { get; set; } = new Dictionary<Vector2i, Map_Tile>();

        private Map_Tile Get_Correct_Map_Tile(Vector2 position) {

            int final_tileSize = tileSize * cellSize;
            Vector2i key = new((int)System.Math.Floor(position.X / final_tileSize), (int)System.Math.Floor(position.Y / final_tileSize));
            key *= final_tileSize;
            key += new Vector2i(final_tileSize / 2, final_tileSize / 3);

            if (!mapTiles.ContainsKey(key)) {

                mapTiles.Add(key, new Map_Tile(key));

                if (Game.Instance.show_performance)
                    DebugData.numOfTiels++;
            }

            mapTiles.TryGetValue(key, out Map_Tile current_tile);
            return current_tile;
        }
    }

    public struct Map_Tile {

        public Vector2 position = new();

        public List<Sprite> background = new();
        public List<Game_Object> staticGameObject = new();
        public List<Body> staticColliders = new();

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
