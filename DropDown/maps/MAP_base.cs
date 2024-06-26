
namespace DropDown.maps {

    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using DropDown.player;
    using DropDown.utility;
    using OpenTK.Mathematics;
    using System;

    internal class Drop_Hole : Game_Object {

        public System.Action enter { get; set; }
        private float last_interaction_time = 0;
        private float reset_time = 1;

        public override void Hit(hitData hit) {
            base.Hit(hit);

            if(hit.hit_object == Game.Instance.player 
                && last_interaction_time < (Game_Time.total - reset_time)) {

                if(!(Game.Instance.get_active_map() is MAP_base))
                    return;
                
                last_interaction_time = Game_Time.total;
                ((MAP_base)(Game.Instance.get_active_map())).start_droping_into_hole();
            }
        }
    }

    public enum map_entry_status {

        None = 0,
        entering = 1,
        inside = 2,
        exeting = 3,
    }

    public class MAP_base : Map {

        protected Cellular_Automata cellular_automata { get; set; }

        protected Texture[] blood_textures = new Texture[4];
        protected Random random;
        protected Vector2 hole_location = new Vector2();
        protected Vector2 hole_location_offset = new Vector2(50, 80);
        protected readonly Texture textureBuffer = Resource_Manager.Get_Texture("assets/textures/terrain.png");
        protected double mapGenerationDuration = 0;
        protected double collisionGenerationDuration = 0;

        protected const int boss_level_itervall = 2;
        protected Vector2i texture_regon_main;
        protected Vector2i texture_regon_detail;
        protected Vector2i texture_regon_detail_small;

        // data for level transition animation
        public map_entry_status map_Entry_Status = map_entry_status.entering;
        private float last_interaction_time = 0;
        private float level_fall_duration = 1;
        //private float buffer_zoom_offset;
        private Character player;
        private Vector2 player_size;
        public void start_droping_into_hole() { map_Entry_Status = map_entry_status.exeting; }
        public override void update(Single deltaTime) {
            base.update(deltaTime);

            if(map_Entry_Status == map_entry_status.exeting) {

                // transition START
                if(last_interaction_time == 0) {

                    player = Game.Instance.player;
                    last_interaction_time = Game_Time.total;
                    Game.Instance.playerController = new PC_empty();
                    ((Drop_Down)Game.Instance).buffer_zoom = Game.Instance.camera.zoom;
                    //buffer_zoom_offset = Game.Instance.camera.zoom_offset;
                    player_size = player.transform.size;
                }

                // transition END
                if(Game_Time.total > last_interaction_time + level_fall_duration)
                    Game.Instance.set_active_map(new MAP_level(++((Drop_Down)Game.Instance).current_drop_level));

                // transition
                Vector2 direction = (this.hole_location + hole_location_offset) - player.transform.position;
                direction.NormalizeFast();
                direction *= ((last_interaction_time + level_fall_duration) - Game_Time.total) * 4;
                player.Add_Linear_Velocity( util.convert_Vector<Box2DX.Common.Vec2>(direction) );

                if((last_interaction_time + level_fall_duration) - Game_Time.total > 0)
                    player.transform.size = player_size * new Vector2(((last_interaction_time + level_fall_duration) - Game_Time.total));

                Game.Instance.camera.transform.position = player.transform.position;
                Game.Instance.camera.Set_Zoom(((Drop_Down)Game.Instance).buffer_zoom * (1 / ((last_interaction_time + level_fall_duration) - Game_Time.total)));

            }

            if(map_Entry_Status == map_entry_status.entering) {

                // transition START
                if(last_interaction_time == 0) {

                    player = Game.Instance.player;
                    last_interaction_time = Game_Time.total;
                    //buffer_zoom = Game.Instance.camera.zoom;
                    //buffer_zoom_offset = Game.Instance.camera.zoom_offset;
                    Game.Instance.playerController = ((Drop_Down)Game.Instance).default_player_controller;
                }

                // transition END
                if(Game_Time.total > last_interaction_time + level_fall_duration) {

                    player.transform.size = new Vector2(100);
                    Game.Instance.camera.zoom = ((Drop_Down)Game.Instance).buffer_zoom;
                    //Game.Instance.camera.zoom_offset = buffer_zoom_offset;

                    map_Entry_Status = map_entry_status.inside;
                    last_interaction_time = 0;
                }

                float time = ((last_interaction_time + level_fall_duration) - Game_Time.total);

                if((last_interaction_time + level_fall_duration) - Game_Time.total > 0)
                    player.transform.size = new Vector2(util.Lerp(100, 400, time));

                Game.Instance.camera.transform.position = player.transform.position;
                Game.Instance.camera.Set_Zoom(util.Lerp(((Drop_Down)Game.Instance).buffer_zoom, 0.1f, time));

            }

        }

        public MAP_base(int dificulty_level, int seed = -1)   {

            if(seed != -1)
                random = new Random(seed);
            else
                random = new Random();

            blood_textures[0] = Resource_Manager.Get_Texture("assets/textures/blood_00.png");
            blood_textures[1] = Resource_Manager.Get_Texture("assets/textures/blood_01.png");
            blood_textures[2] = Resource_Manager.Get_Texture("assets/textures/blood_02.png");
            blood_textures[3] = Resource_Manager.Get_Texture("assets/textures/blood_03.png");

            this.cellSize = 150;
            this.minDistancForCollision = (float)(this.cellSize * this.tileSize);

            int coord_x = (((dificulty_level-1)/boss_level_itervall) * 3);
            texture_regon_main = new Vector2i(coord_x + 2, 5);
            texture_regon_detail = new Vector2i(coord_x + 1, 5);
            texture_regon_detail_small = new Vector2i(coord_x, 5);
            
        }

        public void add_drop_hole(Vector2 location) {

            Add_Sprite(
                new Sprite(
                    new Transform(location, new Vector2(cellSize * 8)),
                    Resource_Manager.Get_Texture("assets/textures/hole.png")));
            Add_Static_Game_Object(
                new Drop_Hole { transform = new Transform(new Vector2(), new Vector2(cellSize * 8)) },
                new Transform(hole_location_offset, new Vector2(-(cellSize * 4))),
                location,
                false,
                true,
                true);
        }

        public void add_blood_splater(Vector2 position) {
            
            this.Add_Sprite(
                new Sprite(
                    new Transform { 
                        position = position, 
                        rotation = (2*float.Pi) * random.NextSingle() },
                    blood_textures[random.Next(blood_textures.Length - 1)]
                    ));
        }


        public override void Draw_Imgui() {
            base.Draw_Imgui();

            if(!Game.Instance.showDebug)
                return;

            if (cellular_automata != null)
                cellular_automata.draw_bit_map_debug_data();
        }


    }
}
