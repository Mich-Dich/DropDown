
namespace DropDown {

    using Core;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    internal class level_hole : Game_Object {

        public override void Hit(hitData hit) {
            base.Hit(hit);

            ((Drop_Down)Game.Instance).set_play_state(Play_State.Playing);

            if(hit.hit_object == Game.Instance.player)
                Game.Instance.set_active_map(new MAP_base());

        }
    }

    internal class MAP_start : Map {

        private const int DefaultCellSize = 100;

        private enum road_direction {
            
            up = 0, 
            down = 1, 
            left = 2, 
            right = 3,
        }

        public MAP_start() {

            this.cellSize = DefaultCellSize;
            this.minDistancForCollision = this.cellSize * this.tileSize;
            
            Generate_Backgound_Tile(100, 100);
            Add_Player(Game.Instance.player, new Vector2(40, -600));

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



            add_road(new Vector2(-120, -600), 10, road_direction.left);
            add_road(new Vector2(40, -760), 10, road_direction.up);
            add_road(new Vector2(40, -440), 6, road_direction.down);

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



        }


    }
}
