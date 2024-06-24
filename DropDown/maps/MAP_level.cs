
namespace DropDown.maps {

    using Core;
    using Core.util;
    using Core.world;
    using DropDown.enemy;
    using DropDown.utility;
    using OpenTK.Mathematics;
    using System;
    using System.Diagnostics;

    public class MAP_level : MAP_base {

        public MAP_level(int dificulty_level, int seed = -1) 
            : base(dificulty_level, seed) {

            cellular_automata = new Cellular_Automata();

            // change default settings for a boss level
            if((dificulty_level % boss_level_itervall) == 0 && dificulty_level != 0) {

                Console.WriteLine($"Boss level");
                cellular_automata.iterations = new int[] { 4, 5, 6, 7 };
            }

            cellular_automata.Generate_Bit_Map();
            Generate_Actual_Map();

            int iteration = 0;
            bool found = false;
            Vector2 player_pos = new Vector2();
            while(!found && iteration < 1000) {

                iteration++;
                player_pos = cellular_automata.find_random_free_positon();
#if DEBUG
                if((hole_location - player_pos).Length < (10 * cellSize)
                    && (hole_location - player_pos).Length > (5 * cellSize))
#else
                if((hole_location - player_pos).Length > (40 * cellSize))
#endif
                    found = true;
            }
            Add_Player(Game.Instance.player, player_pos);
            ((Drop_Down)Game.Instance).set_play_state(DropDown.Game_State.Playing);
            

            //for(int x = 0; x < 1000; x++)
            //    add_blood_splater(cellular_automata.find_random_free_positon());

            const int max_body_count = 490;
            int spanable_count = max_body_count- physicsWorld.GetBodyCount();
#if CANCLE_ENEMY_SPAWN
            spanable_count = 0;
#endif
            Console.WriteLine($"number of enemys: {spanable_count}");

            // spawn enemys
            for(int x = 0; x < (spanable_count * 0.8f); x++)
                spaw_enemy(typeof(CH_small_bug));
            for(int x = 0; x < (spanable_count * 0.2f); x++)
                spaw_enemy(typeof(CH_spider));

            Console.WriteLine($"number of colliders: {physicsWorld.GetBodyCount()}");

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
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, texture_regon_detail_small.X, texture_regon_detail_small.Y),
                            position);
                    }

                    else if(probebilits < 0.2f) {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, texture_regon_detail.X, texture_regon_detail.Y),
                            position);
                    }

                    else {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, texture_regon_main.X, texture_regon_main.Y),
                            position);
                    }

                }
            }


            var Location_buffer = cellular_automata.empty_tile_location[0];
            hole_location = new Vector2(
                ((Location_buffer.X - 4) * cellSize * 8) + (cellSize * 4) - cellSize / 2,
                ((Location_buffer.Y - 4) * cellSize * 8) + (cellSize * 4) - cellSize / 2);

            // Add dungeon entrance
            add_drop_hole(hole_location);

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
                                , false);

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

        private void spaw_enemy(Type enemy_type) {

            if(!typeof(CH_base_NPC).IsAssignableFrom(enemy_type))
                throw new InvalidOperationException($"Type [{enemy_type.Name}] does not implement [I_state] interface.");

            List<Character> newEnemies = new List<Character>();
            CH_base_NPC newEnemy = (CH_base_NPC)Activator.CreateInstance(enemy_type);
            newEnemies.Add(newEnemy);

            int iteration = 0;
            bool found = false;
            Vector2 spawn_pos = new Vector2();
            while(!found && iteration < 1000) {

                iteration++;
                spawn_pos = cellular_automata.find_random_free_positon();
                if((hole_location - spawn_pos).Length > (cellSize * tileSize))
                    found = true;
            }

            this.add_AI_Controller(new AIC_simple(newEnemies));
            Add_Character(newEnemy, spawn_pos, random.NextSingle() * (float.Pi * 2));

        }


    }
}
