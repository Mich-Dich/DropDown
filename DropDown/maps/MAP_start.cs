
namespace DropDown {

    using Core;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    internal class level_hole : Game_Object {

        public override void Hit(hitData hit) {
            base.Hit(hit);

            if(hit.hit_object == Game.Instance.player) {

                Console.WriteLine($"PLAYER HIT LEVEL HOLE");

                Game.Instance.set_active_map(new MAP_base());
            }
        }
    }

    internal class MAP_start : Map {

        private const int DefaultCellSize = 100;

        // Initializes a new instance of the <see cref="MAP_default"/> class.
        public MAP_start() {

            this.cellSize = DefaultCellSize;
            this.minDistancForCollision = this.cellSize * this.tileSize;
            
            Generate_Backgound_Tile(100, 100);
            Add_Player(Game.Instance.player, new Vector2(0, -600));

            // Add dungeon entrance
            Add_Sprite(
                new Sprite(
                    new Transform(new Vector2(), new Vector2(cellSize * 8)),
                    Resource_Manager.Get_Texture("assets/textures/hole.png")));
            Add_Static_Game_Object(
                new level_hole { transform = new Transform(new Vector2(), new Vector2(cellSize * 8)) },
                new Transform(new Vector2(50, 80), new Vector2(- (cellSize * 4))),
                new Vector2(), 
                false, 
                true, 
                true);


            Add_Sprite(
                new Sprite(
                    new Transform(new Vector2(210, -250), new Vector2(220, 160)),
                    Resource_Manager.Get_Texture("assets/textures/sign.png")));



            for(int x = -10; x < 10; x++) {

                add_road_segment(new Vector2(30, -700 + (x * 80) ));
            }

        }

        private void add_road_segment(Vector2 position) {

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

        public override void update(float deltaTime) {



        }


    }
}
