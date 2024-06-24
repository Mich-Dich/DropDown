﻿
namespace DropDown.utility {

    using Core.util;
    using ImGuiNET;
    using OpenTK.Mathematics;
    using System.Diagnostics;

    public class Cellular_Automata {

        public ulong[] bit_map = new ulong[64];
        public List<Vector2> empty_tile_location = new List<Vector2>();
        public int[] iterations = new int[] {4,4,4,4,4};
        public int cellSize = 150;
        public float initalDensity = 0.37f;

        private float tileDisplaySize = 4.5f;
        private double bitMapGenerationDuration = 0;
        private double bitMapGenerationIteration = 0;
        private ulong[] bit_map_buffer = new ulong[64];
        private Random random;

        public Cellular_Automata(int seed = -1) {

            if(seed != -1)
                random = new Random(seed);
            else
                random = new Random();
        }

        // ====================================================================================================================================================================
        // PUBLIC
        // ====================================================================================================================================================================

        public void draw_bit_map_debug_data() {

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
            });
            Imgui_Util.End_Table();

            for(int x = 0; x < iterations.Length; x++) {

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
                for(int y = 63; y >= 0; y--) {

                    ulong currentBit = (bit_map[x] >> y) & 1;

                    draw_list.AddRectFilled(
                        wondopw_pos + new System.Numerics.Vector2(y * tileDisplaySize, x * tileDisplaySize),
                        wondopw_pos + new System.Numerics.Vector2((y + 1) * tileDisplaySize, (x + 1) * tileDisplaySize),
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

            Imgui_Util.Shift_Cursor_Pos(0, 64 * tileDisplaySize + 5);

            Imgui_Util.Begin_Table("map_generation_result");
            Imgui_Util.Add_Table_Row("Bit-map generation duration", $"{bitMapGenerationDuration} ms");
            Imgui_Util.Add_Table_Row("generation until fond place for hole", $"{bitMapGenerationIteration}");
            Imgui_Util.Add_Table_Row("Total duration", $"{bitMapGenerationDuration} ms");
            Imgui_Util.End_Table();

            ImGui.End();
        }

        public void Generate_Bit_Map() {

            Stopwatch stopwatch = new();
            stopwatch.Start();
            bitMapGenerationIteration = 0;

            bool found = false;
            while(found == false && bitMapGenerationIteration < 1000) {

                // Generate random 64x64 Bit-Map
                for(int x = 0; x < 64; x++)
                    bit_map[x] = util.generate_random_UInt64_with_density(initalDensity, random);

                // apply cellular atomita
                for(int x = 0; x < iterations.Length; x++)
                    Iterate_Over_Bit_Map(iterations[x]);

                // check if hole can be placed
                int found_places = 0;
                empty_tile_location = new List<Vector2>();
                for(int y = 0; y < 8; y++) {
                    for(int x = 0; x < 8; x++) {

                        byte[] current_tile = Get8x8_Block(x, y);
                        int count = 0;
                        foreach(byte b in current_tile)
                            count += b;

                        if(count == 0) {

                            found_places++;
                            empty_tile_location.Add(new Vector2(x, y));
                        }
                    }
                }

                found = (found_places > 0);
                bitMapGenerationIteration++;
            }

            if(!found)
                throw new Exception("tryed to find a map that fufills all conditions, but failed");

            stopwatch.Stop();
            bitMapGenerationDuration = stopwatch.Elapsed.TotalMilliseconds;
        }

        public Vector2 find_random_free_positon() {
            
            int iterations = 0;
            bool found = false;
            int offset_y = 0, offset_x = 0;
            while(!found && iterations < 1000) {

                offset_y = random.Next(64);
                offset_x = random.Next(64);
                found = is_coord_in_bit_map_free(offset_x, offset_y);
            }

            if(!found)
                throw new Exception("Failed to find a random empty position");

            return new Vector2((offset_x - 32) * cellSize, (offset_y - 32) * cellSize);
        }
        
        public byte[] Get8x8_Block(int tile_index_x, int tile_index_y) {

            const int loc_tileSize = 8;
            const int totalBits = sizeof(ulong) * 8;
            if(tile_index_x < 0 || tile_index_x > (totalBits / loc_tileSize) || tile_index_y < 0 || tile_index_y > (totalBits / loc_tileSize))
                throw new ArgumentException("Invalid startRow or startCol for extracting 8x8 block.");

            // Iterate over the rows of the 8x8 block
            ulong int_buffer = 0;
            byte[] block = new byte[8];
            for(int x = 0; x < 8; x++) {

                int_buffer = bit_map[(tile_index_y * loc_tileSize) + x];
                block[x] = (byte)((int_buffer >> (tile_index_x * 8)) & 0xFF);
            }

            return block;
        }

        // ====================================================================================================================================================================
        // PRIVATE
        // ====================================================================================================================================================================

        private bool is_coord_in_bit_map_free(int x, int y) { return (bit_map[y] & (UInt64)1 << x) == 0; }

        private void Iterate_Over_Bit_Map(int threshhold = 4) {

            bit_map_buffer = new ulong[64];

            // Determine the number of bits in UInt64 (64 bits)
            const int totalBits = sizeof(ulong) * 8;

            for(int x = 0; x < totalBits; x++) {

                // Loop through each bit position
                for(int y = totalBits - 1; y >= 0; y--) {

                    // Extract bits of current X
                    ulong currentBit = (bit_map[x] >> y) & 1;
                    ulong previousBit = (y > 0) ? ((bit_map[x] >> (y - 1)) & 1) : 1;
                    ulong nextBit = (y < totalBits - 1) ? ((bit_map[x] >> (y + 1)) & 1) : 1;
                    uint combinedBits = (uint)((nextBit << 2) | (currentBit << 1) | previousBit);

                    // Extract bits of u64 one above X
                    ulong upper_buffer = ((x - 1) < 0)? ulong.MaxValue: bit_map[x - 1];
                    ulong upper_currentBit = (upper_buffer >> y) & 1;
                    ulong upper_previousBit = (y > 0) ? ((upper_buffer >> (y - 1)) & 1) : 1;
                    ulong upper_nextBit = (y < totalBits - 1) ? ((upper_buffer >> (y + 1)) & 1) : 1;
                    uint upper_combinedBits = (uint)((upper_nextBit << 2) | (upper_currentBit << 1) | upper_previousBit);

                    // Extract bits of u64 one below X
                    ulong lower_buffer = ((x + 1) > totalBits-1)? ulong.MaxValue: bit_map[x + 1];
                    ulong lower_currentBit = (lower_buffer >> y) & 1;
                    ulong lower_previousBit = (y > 0) ? ((lower_buffer >> (y - 1)) & 1) : 1;
                    ulong lower_nextBit = (y < totalBits - 1) ? ((lower_buffer >> (y + 1)) & 1) : 1;
                    uint lower_combinedBits = (uint)((lower_nextBit << 2) | (lower_currentBit << 1) | lower_previousBit);

                    int count = Count_Ones_Exclude_Middle(combinedBits) + Count_Ones(upper_combinedBits) + Count_Ones(lower_combinedBits);

                    if(count >= threshhold) {

                        ulong mask = (ulong)1 << y; // Calculate the mask based on MSB index (63 - bitIndex)
                        bit_map_buffer[x] |= mask;
                    }
                }
            }
            bit_map = bit_map_buffer;
        }



        private void Log_U64(ulong number) {

            string binaryRepresentation = Convert.ToString((long)number, 2).PadLeft(64, '0');

            // Replace '0' with ' ' and '1' with 'X'
            char[] binaryChars = binaryRepresentation.Select(c => c == '0' ? ' ' : 'X').ToArray();
            string formattedBinary = new(binaryChars);

            Console.WriteLine($"Map: |{formattedBinary}|");
        }

        // Counts the number of '1's in a 3-bit number
        private int Count_Ones(uint num) {

            int count = 0;
            for(int i = 0; i < 3; i++) {
                if((num & (1 << i)) != 0)
                    count++;
            }
            return count;
        }

        // Counts the number of '1's in the leftmost and rightmost bits of a 3-bit number
        private int Count_Ones_Exclude_Middle(uint num) {

            int count = 0;
            for(int i = 0; i <= 2; i += 2) {
                if((num & (1 << i)) != 0)
                    count++;
            }
            return count;
        }


    }
}
