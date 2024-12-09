
namespace DropDown.maps {
    using Core;
    using Core.Particles;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    internal class MAP_start : MAP_base {

        private const int DefaultCellSize = 100;
        private float shockwaveTimeStamp = 0f;

        private enum road_direction {
            
            up = 0, 
            down = 1, 
            left = 2, 
            right = 3,
        }


        private Func<Vector2> VelocityFunction;
        private Func<float> SizeFunction;
        private Func<float> RotationFunction;
        private ColorGradient ColorGradient;
        public bool IsActive { get; private set; } = true;
        private Func<bool> IsAffectedByForcesFunction;

        // At the class level, define these constants
        private const float PARTICLE_BASE_SPEED = 5.0f;
        private const float PARTICLE_SIZE_START = 20.0f;
        private const float PARTICLE_SIZE_END = 5.0f;
        private const float ROTATION_SPEED = 45.0f; // degrees per second

        public MAP_start()
            : base(0) {

            this.cellSize = DefaultCellSize;
            this.minDistancForCollision = this.cellSize * this.tileSize;

            Generate_Backgound_Tile(100, 100);
            Add_Player(Game.Instance.player, new Vector2(40, -600));


            // Add dungeon entrance
            add_drop_hole(new Vector2());

            Add_Sprite(
                new Sprite(
                    new Transform(new Vector2(210, -250), new Vector2(220, 160)),
                    Resource_Manager.Get_Texture("assets/textures/sign.png")));

            add_road(new Vector2(-120, -600), 10, road_direction.left);
            add_road(new Vector2(40, -760), 10, road_direction.up);
            add_road(new Vector2(40, -440), 6, road_direction.down);

            ColorGradient = new ColorGradient();
            ColorGradient.AddColor(0.0f, new Vector4(0.0f, 0.8f, 1.0f, 1.0f)); // Bright cyan
            ColorGradient.AddColor(0.3f, new Vector4(0.0f, 0.6f, 1.0f, 0.7f)); // Medium blue
            ColorGradient.AddColor(0.6f, new Vector4(0.0f, 0.4f, 1.0f, 0.4f)); // Darker blue
            ColorGradient.AddColor(1.0f, new Vector4(0.0f, 0.0f, 0.5f, 0.0f)); // Dark blue, fade out

            // Define the functions
            VelocityFunction = () => {
                float angle = Random.Shared.NextSingle() * MathHelper.TwoPi;
                return new Vector2(
                    MathF.Cos(angle) * PARTICLE_BASE_SPEED,
                    MathF.Sin(angle) * PARTICLE_BASE_SPEED
                );
            };

            SizeFunction = () => { return MathHelper.Lerp(PARTICLE_SIZE_START, PARTICLE_SIZE_END, Random.Shared.NextSingle()); };

            RotationFunction = () => { return ROTATION_SPEED * (Random.Shared.NextSingle() * 2 - 1); };                                 // Random rotation between -45 and 45 degrees

            IsAffectedByForcesFunction = () => true;                                                                                    // Particles affected by forces like gravity

            //AOE_spell test = new AOE_spell(new Vector2(600,0) );
            //Add_Game_Object(test);


            //this.Add_Game_Object(new Game_Object(
            //        new Transform(
            //            new Vector2(0)
            //        ))
            //        .Add_Collider(new Collider(Collision_Shape.Circle, Collision_Type.bullet))
            //        .Set_Mobility(Mobility.DYNAMIC)
            //        .Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/sign.png")))
            //    );
        }

    private void add_road(Vector2 start_position, int length, road_direction direction) {

            for(int x = 0; x < length; x++) {

                Vector2 offset;

                switch(direction) {
                    case road_direction.up:
                        add_road_segment_vertical(start_position + new Vector2(0, -(x * 80)));
                        break;
                    case road_direction.down:
                        add_road_segment_vertical(start_position + new Vector2(0, (x * 80)));
                        break;
                    case road_direction.left:
                        add_road_segment_horicontal(start_position + new Vector2(-(x * 80), 0));
                        break;
                    case road_direction.right:
                        add_road_segment_horicontal(start_position + new Vector2((x * 80), 0));
                        break;
                    default:
                        offset = new Vector2(0);
                        break;
                }

            }

        }

        private void add_road_segment_vertical(Vector2 position) {

            Add_Background_Sprite(
                new Sprite(
                    new Transform(null, new Vector2(80)),
                    Resource_Manager.Get_Texture("assets/textures/terrain.png")).
                Select_Texture_Region(32, 64, 0, 42),
                position + new Vector2(-80, 0),
                false);

            Add_Background_Sprite(
                new Sprite(
                    new Transform(null, new Vector2(80)),
                    Resource_Manager.Get_Texture("assets/textures/terrain.png")).
                Select_Texture_Region(32, 64, 1, 42),
                position + new Vector2(0, 0),
                false);

            Add_Background_Sprite(
                new Sprite(
                    new Transform(null, new Vector2(80)),
                    Resource_Manager.Get_Texture("assets/textures/terrain.png")).
                Select_Texture_Region(32, 64, 2, 42),
                position + new Vector2(80, 0),
                false);

        }

        private void add_road_segment_horicontal(Vector2 position) {

            Add_Background_Sprite(
                new Sprite(
                    new Transform(null, new Vector2(80)),
                    Resource_Manager.Get_Texture("assets/textures/terrain.png")).
                Select_Texture_Region(32, 64, 1, 41),
                position + new Vector2(0, -80),
                false);

            Add_Background_Sprite(
                new Sprite(
                    new Transform(null, new Vector2(80)),
                    Resource_Manager.Get_Texture("assets/textures/terrain.png")).
                Select_Texture_Region(32, 64, 1, 42),
                position + new Vector2(0, 0),
                false);

            Add_Background_Sprite(
                new Sprite(
                    new Transform(null, new Vector2(80)),
                    Resource_Manager.Get_Texture("assets/textures/terrain.png")).
                Select_Texture_Region(32, 64, 1, 43),
                position + new Vector2(0, 80),
                false);

        }

        public override void update(float deltaTime) {
            base.update(deltaTime);



            if(Game_Time.total - shockwaveTimeStamp >= 1.0f) {

                Vector2 position = Vector2.Zero;

                Console.WriteLine($"Trying to add emitter");
                this.particleSystem.AddEmitter(new Emitter(
                    new Vector2(), 50, true, 50, VelocityFunction, SizeFunction, RotationFunction, ColorGradient, IsAffectedByForcesFunction
                    ));

                // Reset the timestamp
                shockwaveTimeStamp = Game_Time.total;
            }


        }


    }
}
