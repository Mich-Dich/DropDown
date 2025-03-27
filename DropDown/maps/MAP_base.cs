
namespace DropDown {

    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using DropDown.enemy;
    using DropDown.maps;
    using DropDown.utility;
    using OpenTK.Mathematics;
    using System;
    using System.Diagnostics;



    internal class Drop_Hole : Game_Object {

        public Action enter { get; set; }

        public override void Hit(hitData hit) {
            base.Hit(hit);

            if(hit.hit_object == Game.Instance.player)
                Game.Instance.set_active_map(new MAP_base());

        }
    }

    public class MAP_base : Map {
    
        private Type[] enemy_types = new Type[] { typeof(CH_small_bug), typeof(CH_spider) };

        private Texture[] blood_textures = new Texture[4];
        private Random random;
        private Cellular_Automata cellular_automata;
        private Vector2 hole_location = new Vector2();
        private readonly Texture textureBuffer = Resource_Manager.Get_Texture("assets/textures/terrain.png");
        private double mapGenerationDuration = 0;
        private double collisionGenerationDuration = 0;

        public MAP_base(int seed = -1)   {

            ((Drop_Down)Game.Instance).current_level++;
            int map_level = ((Drop_Down)Game.Instance).current_level;

            ((Drop_Down)Game.Instance).set_play_state(Play_State.Playing);

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

            cellular_automata = new Cellular_Automata();

            if (map_level % 5 == 0)                                     // every 5 levels is a boss room
                    cellular_automata.iterations = new int[]{ 4, 4, 6, 5, 4 };

            cellular_automata.Generate_Bit_Map();

            Generate_Actual_Map();

            // always spawn 100 enemys      always two types    ratio depends on map level
            for(int x = 0; x < ((map_level % 5) * 20); x++)
                spaw_enemy(typeof(CH_small_bug));
            for(int x = 0; x < (((5 - map_level) % 5) * 20); x++)
                spaw_enemy(typeof(CH_spider));

            int iteration = 0;
            bool found = false;
            Vector2 player_pos = new Vector2();
            while(!found && iteration < 1000) {

                iteration++;
                player_pos = cellular_automata.find_random_free_positon();
                if((hole_location - player_pos).Length > (40 * cellSize))
                    found = true;
            }

            //spawn random sparkel
            //for(int x = 0; x < 50; x++) {
            //    this.Add_Background_Sprite(
            //        new Sprite { transform = new Transform { size = new Vector2(300) } }.set_animation("assets/animation/spell_00.png", 5, 6, true, false, 20, true),
            //        cellular_automata.find_random_free_positon(),
            //        false);
            //}

            //for(int x = 0; x < 1000; x++)
            //    add_blood_splater(find_random_free_positon());

            Add_Player(Game.Instance.player, player_pos);
        }


        private void populate_enemys() {



        }


        public void add_blood_splater(Vector2 position) {
            
            this.Add_Sprite(new Sprite(new Transform { position = position, rotation = (2*float.Pi) * random.NextSingle() }, blood_textures[random.Next(blood_textures.Length - 1)]));
        }


        public override void Draw_Imgui() {
            base.Draw_Imgui();

            if(!Game.Instance.showDebug)
                return;

            cellular_automata.draw_bit_map_debug_data();
        }

        // ====================================================================================================================================================================
        // PRIVATE
        // ====================================================================================================================================================================

        private void Generate_Actual_Map() {

            Stopwatch stopwatch = new();
            stopwatch.Start();

            // --------------------------- spawn sprites --------------------------- 
            Random rnd = new();
            Force_Clear_mapTiles();
            const int totalBits = sizeof(ulong) * 8;
            Vector2 position;
            for(int x = 0; x < totalBits; x++) {
                for(int y = 0; y < totalBits; y++) {

                    // Extract bits of current X
                    ulong currentBit = (cellular_automata.bit_map[x] >> y) & 1;

                    if(currentBit == 1)
                        continue;

                    position = new Vector2((y - totalBits / 2) * this.cellSize, (x - totalBits / 2) * this.cellSize);
                    double probebilits = rnd.NextDouble();
                    if(probebilits < 0.05f) {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, 3, 5),
                            position);
                    }

                    else if(probebilits < 0.2f) {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, 4, 5),
                            position);
                    }
                    
                    else {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, 5, 5),
                            position);
                    }

                }
            }


            var Location_buffer = cellular_automata.empty_tile_location[0];
            hole_location = new Vector2(
                ((Location_buffer.X - 4) * cellSize * 8) + (cellSize * 4) - cellSize / 2,
                ((Location_buffer.Y - 4) * cellSize * 8) + (cellSize * 4) - cellSize / 2);

            // Add dungeon entrance
            Add_Sprite(
                new Sprite(
                    new Transform(hole_location, new Vector2(cellSize * 8)),
                    Resource_Manager.Get_Texture("assets/textures/hole.png")));
            Add_Static_Game_Object(
                new Drop_Hole { transform = new Transform(new Vector2(), new Vector2(cellSize * 8)) },
                new Transform(new Vector2(50, 80), new Vector2(-(cellSize * 4))),
                hole_location,
                false,
                true,
                true);

            stopwatch.Stop();
            mapGenerationDuration = stopwatch.Elapsed.TotalMilliseconds;
            stopwatch.Restart();

            // --------------------------- generate collision --------------------------- 

            for(int x = 0; x < totalBits / tileSize; x++) {
                for(int y = 0; y < totalBits / tileSize; y++) {

                    Vector2 tile_offset = new(
                        (x- (totalBits / tileSize/2)) * (this.tileSize * this.cellSize),
                        (y- (totalBits / tileSize/2)) * (this.tileSize * this.cellSize));

                    // Display each byte in the array as a binary string
                    byte[] current_tile = cellular_automata.Get8x8_Block(x, y);
                    for(int z = 0; z < current_tile.Length; z++) {

                        // repeat untill all bit are 0 in this byte (auto skips empty bytes)
                        while(current_tile[z] != 0) {

                            // --------------- find block of ones --------------- 
                            byte buffer = current_tile[z];
                            int firstOneIndex = -1;
                            int column_counter = 0;                               // used as X value of size
                            int row_counter = 1;                                // used as Y value of size

                            // --------------- calc max size.X of collider --------------- 
                            for(int i = 0; i < 8; i++) {
                                if((buffer & (1 << i)) == 0)
                                    continue;

                                firstOneIndex = i;
                                column_counter = util.Count_Number_Of_Following_Ones(buffer, firstOneIndex);
                                break;
                            }
                            
                            // --------------- calc max size.Y of collider --------------- 
                            int questionable_count = 0;
                            do {

                                if(z + 1 > 7)
                                    break;
                                
                                questionable_count = util.Count_Number_Of_Following_Ones(current_tile[z + row_counter], firstOneIndex);
                                if(questionable_count < column_counter)
                                    break;

                                row_counter++;

                            } while((z + row_counter) < current_tile.Length && questionable_count >= column_counter);
                            //Console.WriteLine($"size of collider: {column_counter}/{row_counter}");

                            Vector2 collider_tile_offset = new(
                                (this.cellSize * -0.5f) + firstOneIndex * this.cellSize,
                                (this.cellSize * -0.5f) + z * this.cellSize
                                );

                            // --------------- add collider --------------- 

                            this.add_static_collider_AAABB(
                                new Transform(
                                    new Vector2(column_counter * this.cellSize / 2, row_counter * this.cellSize / 2) + tile_offset + collider_tile_offset,
                                    new Vector2(column_counter * this.cellSize, row_counter * this.cellSize))
                                ,false);

                            // --------------- set used values to 0 --------------- 
                            byte reset_mask = 0;
                            byte mask = (byte)((1 << column_counter) - 1);      // Creates a sequence of 'count' ones
                            mask <<= firstOneIndex;                             // Shift the mask to the left by 'start_index' positions
                            reset_mask |= mask;

                            for(int i = 0; i < row_counter; i++) 
                                current_tile[z + i] = (byte)(current_tile[z + i] & ~reset_mask);

                        }
                    }
                }
            }

            stopwatch.Stop();
            collisionGenerationDuration = stopwatch.Elapsed.TotalMilliseconds;
        }


        private void spaw_enemy(Type enemy_type)  {

            if (!typeof(CH_base_NPC).IsAssignableFrom(enemy_type))
                throw new InvalidOperationException($"Type [{enemy_type.Name}] does not implement [I_state] interface.");

            List<Character> newEnemies = new List<Character>();
            CH_base_NPC newEnemy = (CH_base_NPC)Activator.CreateInstance(enemy_type);
            newEnemies.Add(newEnemy);

            this.add_AI_Controller(new AIC_simple(newEnemies));

            Add_Character(newEnemy, cellular_automata.find_random_free_positon(), random.NextSingle() * (float.Pi * 2));
        }



    }
}
