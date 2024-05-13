﻿
namespace DropDown {

    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using Core.world.map;
    using DropDown.enemy;
    using ImGuiNET;
    using OpenTK.Mathematics;
    using System.Diagnostics;

    public class Base_Map : Map {

        public Base_Map() {

            //this.generate_backgound_tile(50, 30);

            //Physics_Material ateroid_phys_mat = new Physics_Material(0.0f, 0.1f);
            //Texture ateroid_texture = resource_manager.get_texture("assets/textures/muzzle_flash.jpg");
            //this.add_game_object(
            //    new game_object(new Vector2(550, -350), new Vector2(300, 100))
            //        .set_sprite(ateroid_texture)
            //        .Set_Mobility(mobility.STATIC)
            //        .add_collider(new collider(Collision_Shape.Square)));


            //this.add_character(new character().add_collider(new collider()), new Vector2(-300));

            this.cellSize = 150;
            this.minDistancForCollision = (float)(this.cellSize * this.tileSize);
            Generate_Bit_Map();
            Generate_Actual_Map();

        }

        public void Generate_Map_From_Bit_Map() {
            
        }

        public override void Draw_Imgui() {

            Imgui_Diaplay_Level_Bit_Map();
        }

        private void Imgui_Diaplay_Level_Bit_Map() {


            if(!Game.Instance.showDebug)
                return;

            if(!ImGui.Begin("level_bit_map"))
                return;

            Imgui_Util.Title("Data for map:");

            Imgui_Util.Begin_Table("bit_map_generation_data");
            Imgui_Util.Add_Table_Row("inital density", ref initalDensity, 0.0002f, 0f, 1f);
            Imgui_Util.Add_Table_Row("tile display size", ref tileDisplaySize, 0.1f, 3, 10);
            Imgui_Util.Add_Table_Row("Iterations count", () => { 
                ImGui.Text($"{iterations.Length}");
                ImGui.SameLine();
                if(ImGui.Button("+##add to iterations")) {

                    int[] buffer = new int[iterations.Length + 1];
                    Array.Copy(iterations, buffer, iterations.Length);
                    buffer[buffer.Length - 1] = 4;
                    iterations = buffer;
                }
                ImGui.SameLine();
                if(ImGui.Button("-##add to iterations")) {

                    int[] buffer = new int[iterations.Length - 1];
                    Array.Copy(iterations, buffer, iterations.Length - 1);
                    iterations = buffer;
                }
            } );
            Imgui_Util.End_Table();

            for (int x = 0; x < iterations.Length; x++) {
                
                ImGui.VSliderInt($"##int{x}", new System.Numerics.Vector2(18, 60), ref iterations[x], 0, 8);
                ImGui.SameLine();
            }
            
            if(ImGui.Button("Regenerate")) {
                Generate_Bit_Map();
            }
            
            Imgui_Util.Title("Map generated by cellular automata:");

            ImDrawListPtr draw_list = ImGui.GetWindowDrawList();
            var wondopw_pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new System.Numerics.Vector2(0, ImGui.GetScrollY());
            uint col_white = ImGui.GetColorU32(new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 1));
            uint col_black = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.9f, 0.9f, 1));

            // display bit map
            for(int x = 0; x < 64; x++) {
                for(int y = 63; y >= 0 ; y--) {

                    ulong currentBit = (floorLayout[x] >> y) & 1;

                    draw_list.AddRectFilled(
                        wondopw_pos + new System.Numerics.Vector2(y * tileDisplaySize, x * tileDisplaySize),
                        wondopw_pos + new System.Numerics.Vector2((y + 1) * tileDisplaySize, (x+1) * tileDisplaySize),
                        (currentBit == 0) ? col_black : col_white);
                }
            }

            // display bit map
            uint col_red = ImGui.GetColorU32(new System.Numerics.Vector4(0.9f, 0.2f, 0.2f, 1));
            for(int x = 1; x < 8; x++) {

                // Draw X-Axis Line
                draw_list.AddLine(
                    wondopw_pos + new System.Numerics.Vector2(0, x * tileDisplaySize * 8),
                    wondopw_pos + new System.Numerics.Vector2(64 * tileDisplaySize, x * tileDisplaySize * 8),
                    col_red);

                // Draw Y-Axis Line
                draw_list.AddLine(
                    wondopw_pos + new System.Numerics.Vector2(x * tileDisplaySize * 8, 0),
                    wondopw_pos + new System.Numerics.Vector2(x * tileDisplaySize * 8, 64 * tileDisplaySize),
                    col_red);

            }


            // show player position
            draw_list.AddRectFilled(
                wondopw_pos + new System.Numerics.Vector2(32 * tileDisplaySize, 32 * tileDisplaySize),
                wondopw_pos + new System.Numerics.Vector2((32 + 1) * tileDisplaySize, (32 + 1) * tileDisplaySize),
                ImGui.GetColorU32(new System.Numerics.Vector4(0.2f, 0.9f, 0.2f, 1)));

            Imgui_Util.Shift_Cursor_Pos(0,64 * tileDisplaySize + 5);

            Imgui_Util.Begin_Table("map_generation_result");
            Imgui_Util.Add_Table_Row("Bit-map generation duration", $"{bitMapGenerationDuration} ms");
            Imgui_Util.Add_Table_Row("convert to actual map duration", $"{mapGenerationDuration} ms");
            Imgui_Util.Add_Table_Row("collision generation", $"{collisionGenerationDuration} ms");
            Imgui_Util.Add_Table_Row("generation until start_pos empty", $"{bitMapGenerationIteration}");
            Imgui_Util.Add_Table_Row("Total duration", $"{bitMapGenerationDuration + mapGenerationDuration + collisionGenerationDuration} ms");
            Imgui_Util.End_Table();

            if(ImGui.Button("Spawn Enamy"))
                spaw_enemy();

            if(ImGui.Button("Generate actual map from bit-map"))
                Generate_Actual_Map();

            ImGui.End();
        }

        private void Generate_Actual_Map() {

            Stopwatch stopwatch = new();
            stopwatch.Start();

            // --------------------------- spawn sprites --------------------------- 
            Random rnd = new();
            Force_Clear_mapTiles();
            const int totalBits = sizeof(ulong) * 8;
            for(int x = 0; x < totalBits; x++) {
                for(int y = 0; y < totalBits; y++) {

                    // Extract bits of current X
                    ulong currentBit = (floorLayout[x] >> y) & 1;

                    if(currentBit == 1)
                        continue;


                    double probebilits = rnd.NextDouble();
                    if(probebilits < 0.05f) {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, 3, 58 ),
                            new Vector2((y - totalBits/2) * this.cellSize, (x - totalBits/2) * this.cellSize));
                    }

                    else if(probebilits < 0.2f) {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, 4, 58),
                            new Vector2((y - totalBits / 2) * this.cellSize, (x - totalBits / 2) * this.cellSize));
                    }
                    
                    else {

                        this.Add_Background_Sprite(
                            new Sprite(textureBuffer).Select_Texture_Region(32, 64, 5, 58),
                            new Vector2((y - totalBits / 2) * this.cellSize, (x - totalBits / 2) * this.cellSize));
                    }

                }
            }

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
                    byte[] current_tile = Get8x8_Block(x, y);
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
                                column_counter = Count_Number_Of_Following_Ones(buffer, firstOneIndex);
                                break;
                            }
                            
                            // --------------- calc max size.Y of collider --------------- 
                            int questionable_count = 0;
                            do {

                                if(z + 1 > 7)
                                    break;
                                
                                questionable_count = Count_Number_Of_Following_Ones(current_tile[z + row_counter], firstOneIndex);
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

                            //this.Add_Static_Game_Object(
                            //    new Game_Object(
                            //        null,
                            //        new Vector2(column_counter * this.cell_size, row_counter * this.cell_size))
                            //            .Add_Collider(new Collider(collision_shape.Square)),
                            //    new Vector2((column_counter * this.cell_size) / 2, (row_counter * this.cell_size) / 2) + tile_offset + collider_tile_offset,
                            //    false);

                            // --------------- set used values to 0 --------------- 
                            byte reset_mask = SetBits(firstOneIndex, column_counter);
                            for(int i = 0; i < row_counter; i++) 
                                current_tile[z + i] = (byte)(current_tile[z + i] & ~reset_mask);

                        }
                    }
                }
            }

            stopwatch.Stop();
            collisionGenerationDuration = stopwatch.Elapsed.TotalMilliseconds;
        }

        public void spaw_enemy() {


            Random random = new Random();

            bool found = false;
            int offset_y = 0, offset_x = 0;

            while(!found) {

                offset_y = random.Next(6);
                offset_x = random.Next(6);
                found = is_coord_in_bit_map_free(29 + offset_x, 29 + offset_y);
            }
            
            this.Add_Character(new Base_Enemy(),
                new Vector2((offset_x - 3) * this.cellSize, (offset_y - 3) * this.cellSize));

        }





















        private static byte SetBits(int start_index, int count) {

            byte result = 0;
            byte mask = (byte)((1 << count) - 1);   // Creates a sequence of 'count' ones
            mask <<= start_index;                   // Shift the mask to the left by 'start_index' positions
            result |= mask;

            return result;
        }

        private int Count_Number_Of_Following_Ones(byte target, int index_of_first_one) {

            int count = 0;
            byte buffer = target;
            byte mask = 0x01; // Start with the least significant bit (LSB) mask
            mask <<= index_of_first_one;
            int buffer_index = index_of_first_one;

            // Iterate over each bit of the byte value
            while((buffer & mask) != 0 && buffer_index < 8) {
                count++; // Increment the count for each '1' encountered
                mask <<= 1; // Shift the mask to the left to check the next bit
            }
            return count;
        }

        private byte[] Get8x8_Block(int tile_index_x, int tile_index_y) {

            const int loc_tileSize = 8;
            const int totalBits = sizeof(ulong) * 8;
            if(tile_index_x < 0 || tile_index_x > (totalBits/loc_tileSize) || tile_index_y < 0 || tile_index_y > (totalBits/loc_tileSize)) 
                throw new ArgumentException("Invalid startRow or startCol for extracting 8x8 block.");

            // Iterate over the rows of the 8x8 block
            ulong int_buffer = 0;
            byte[] block = new byte[8];
            for(int x = 0; x < 8; x++) {

                int_buffer = floorLayout[(tile_index_y*loc_tileSize) + x];
                block[x] = (byte)((int_buffer >> (tile_index_x * 8)) & 0xFF);
            }

            return block;
        }


        private void Generate_Bit_Map() {

            Stopwatch stopwatch = new();
            stopwatch.Start();
            bitMapGenerationIteration = 0;

            bool found = false;
            while(found == false && bitMapGenerationIteration < 1000) {

                // Call the method or perform the process you want to measure
                Generate_Random_64x64_Bit_Map();
                for(int x = 0; x < iterations.Length; x++) {

                    //Console.WriteLine($"ITERATION: {x}");
                    Iterate_Over_Bit_Map(iterations[x]);
                }

<<<<<<< HEAD
                found = is_coord_in_bit_map_free(32, 32);
=======
                ulong targetUInt64 = floorLayout[32];
                ulong mask = (ulong)1 << 32;

                found = (targetUInt64 & mask) == 0;
>>>>>>> ad36c3a23732774a4c63254368e4374395eb69c4
                bitMapGenerationIteration++;
                Console.WriteLine($"player loc is empty: {found}");
            }

            stopwatch.Stop();
            bitMapGenerationDuration = stopwatch.Elapsed.TotalMilliseconds;

        }

        private bool is_coord_in_bit_map_free(int x, int y) {

            UInt64 targetUInt64 = floorLayout[y];
            UInt64 mask = (UInt64)1 << x;

            return (targetUInt64 & mask) == 0;
        }

        private void Generate_Random_64x64_Bit_Map() {

            for(int x = 0; x < 64; x++) {

                floorLayout[x] = Generate_Eandom_UInt64_With_Density(initalDensity);
            }

        }

        private void Iterate_Over_Bit_Map(int threshhold = 4) {

            floorLayoutBuffer = new ulong[64];

            // Determine the number of bits in UInt64 (64 bits)
            const int totalBits = sizeof(ulong) * 8;

            for(int x = 0;x < totalBits;x++) {

                // Loop through each bit position
                for(int y = totalBits - 1; y >= 0; y--) {

                    // Extract bits of current X
                    ulong currentBit = (floorLayout[x] >> y) & 1;
                    ulong previousBit = (y > 0) ? ((floorLayout[x] >> (y - 1)) & 1) : 1;
                    ulong nextBit = (y < totalBits - 1) ? ((floorLayout[x] >> (y + 1)) & 1) : 1;
                    uint combinedBits = (uint)((nextBit << 2) | (currentBit << 1) | previousBit);

                    // Extract bits of u64 one above X
                    ulong upper_buffer = ((x - 1) < 0)? ulong.MaxValue: floorLayout[x - 1];
                    ulong upper_currentBit = (upper_buffer >> y) & 1;
                    ulong upper_previousBit = (y > 0) ? ((upper_buffer >> (y - 1)) & 1) : 1;
                    ulong upper_nextBit = (y < totalBits - 1) ? ((upper_buffer >> (y + 1)) & 1) : 1;
                    uint upper_combinedBits = (uint)((upper_nextBit << 2) | (upper_currentBit << 1) | upper_previousBit);

                    // Extract bits of u64 one below X
                    ulong lower_buffer = ((x + 1) > totalBits-1)? ulong.MaxValue: floorLayout[x + 1];
                    ulong lower_currentBit = (lower_buffer >> y) & 1;
                    ulong lower_previousBit = (y > 0) ? ((lower_buffer >> (y - 1)) & 1) : 1;
                    ulong lower_nextBit = (y < totalBits - 1) ? ((lower_buffer >> (y + 1)) & 1) : 1;
                    uint lower_combinedBits = (uint)((lower_nextBit << 2) | (lower_currentBit << 1) | lower_previousBit);

                    int count = Count_Ones_Exclude_Middle(combinedBits) + Count_Ones(upper_combinedBits) + Count_Ones(lower_combinedBits);

                    if(count >= threshhold) {

                        ulong mask = (ulong)1 << y; // Calculate the mask based on MSB index (63 - bitIndex)
                        floorLayoutBuffer[x] |= mask;
                    }

                    //Console.Write($"{count} ");

                }
                //Console.WriteLine();
            }


            //for(int x = 0; x < 64; x++)
            //    Log_U64(bit_map_buffer[x]);

            //Console.WriteLine();
            floorLayout = floorLayoutBuffer;
        }

        // imgui Window
        private float tileDisplaySize = 4.5f;
        private double bitMapGenerationDuration = 0;
        private double bitMapGenerationIteration = 0;
        private double mapGenerationDuration = 0;
        private double collisionGenerationDuration = 0;

        private float initalDensity = 0.37f;
        private int[] iterations = new int[] {4,4,4,4,4};
        private ulong[] floorLayoutBuffer = new ulong[64];
        private ulong[] floorLayout = new ulong[64];
        private readonly Texture textureBuffer = Resource_Manager.Get_Texture("assets/textures/terrain.png");

        private void Log_U64(ulong number) {

            string binaryRepresentation = Convert.ToString((long)number, 2).PadLeft(64, '0');

            // Replace '0' with ' ' and '1' with 'X'
            char[] binaryChars = binaryRepresentation.Select(c => c == '0' ? ' ' : 'X').ToArray();
            string formattedBinary = new(binaryChars);

            Console.WriteLine($"Map: |{formattedBinary}|");
        }


        // Function to count the number of '1's in a 3-bit number
        private static int Count_Ones(uint num) {

            int count = 0;
            for(int i = 0; i < 3; i++) {
                if((num & (1 << i)) != 0)
                    count++;
            }
            return count;
        }

        // Function to count the number of '1's in the leftmost and rightmost bits of a 3-bit number
        private static int Count_Ones_Exclude_Middle(uint num) {

            int count = 0;
            for(int i = 0; i <= 2; i += 2) {
                if((num & (1 << i)) != 0) 
                    count++;
            }
            return count;
        }

        // Function to generate a UInt64 value with specified density of bits flipped
        private static ulong Generate_Eandom_UInt64_With_Density(double density) {

            Random random = new();
            ulong result = 0;
            for(int i = 0; i < 64; i++) {
            
                double randomValue = random.NextDouble();
                if(randomValue < density)
                    result |= (ulong)1 << i;
            }
            return result;
        }

    }
}
