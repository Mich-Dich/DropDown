using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;
using System;

namespace Hell {

    public class base_map : map {

        public base_map() {

            this.generate_backgound_tile(50, 30);

            physics_material ateroid_phys_mat = new physics_material(0.0f, 0.0f, 0.1f);
            
            Texture ateroid_texture = resource_manager.get_texture("assets/textures/muzzle_flash.jpg");
            this.add_game_object(
                new game_object(new Vector2(550, -350), new Vector2(300, 100))
                    .set_sprite(ateroid_texture)
                    .set_mobility(mobility.STATIC)
                    .add_collider(new collider(collision_shape.Square)));


            this.add_character(new character());

            
            generate_random_64x64_bit_map();
            for(int x = 0; x < 5; x++)
                iterate_over_bit_map();


        }


        private void generate_random_64x64_bit_map() {

            for(int x = 0; x < 64; x++) {

                float density = 0.32f;
                floor_layout[x] = generate_random_UInt64_with_density(density);
                log_u64(floor_layout[x]);
            }

        }

        private void iterate_over_bit_map() {

            UInt64[] bit_map_buffer = new UInt64[64];

            // Determine the number of bits in UInt64 (64 bits)
            const int totalBits = sizeof(UInt64) * 8;

            for(int x = 0;x < totalBits;x++) {

                // Loop through each bit position
                for(int i = totalBits - 1; i >= 0; i--) {

                    // Extract bits of current X
                    UInt64 currentBit = (floor_layout[x] >> i) & 1;
                    UInt64 previousBit = (i > 0) ? ((floor_layout[x] >> (i - 1)) & 1) : 1;
                    UInt64 nextBit = (i < totalBits - 1) ? ((floor_layout[x] >> (i + 1)) & 1) : 1;
                    UInt32 combinedBits = (UInt32)((nextBit << 2) | (currentBit << 1) | previousBit);

                    // Extract bits of u64 one above X
                    UInt64 upper_buffer = ((x - 1) < 0)? UInt64.MaxValue: floor_layout[x - 1];
                    UInt64 upper_currentBit = (upper_buffer >> i) & 1;
                    UInt64 upper_previousBit = (i > 0) ? ((upper_buffer >> (i - 1)) & 1) : 1;
                    UInt64 upper_nextBit = (i < totalBits - 1) ? ((upper_buffer >> (i + 1)) & 1) : 1;
                    UInt32 upper_combinedBits = (UInt32)((upper_nextBit << 2) | (upper_currentBit << 1) | upper_previousBit);

                    // Extract bits of u64 one below X
                    UInt64 lower_buffer = ((x + 1) > totalBits-1)? UInt64.MaxValue: floor_layout[x + 1];
                    UInt64 lower_currentBit = (lower_buffer >> i) & 1;
                    UInt64 lower_previousBit = (i > 0) ? ((lower_buffer >> (i - 1)) & 1) : 1;
                    UInt64 lower_nextBit = (i < totalBits - 1) ? ((lower_buffer >> (i + 1)) & 1) : 1;
                    UInt32 lower_combinedBits = (UInt32)((lower_nextBit << 2) | (lower_currentBit << 1) | lower_previousBit);

                    int count = count_ones_exclude_middle(combinedBits) + count_ones(upper_combinedBits) + count_ones(lower_combinedBits);

                    if(count >= 4) {

                        UInt64 mask = (UInt64)1 << i; // Calculate the mask based on MSB index (63 - bitIndex)
                        bit_map_buffer[x] |= mask;
                    }

                    //Console.Write($"{count} ");

                }
                //Console.WriteLine();
            }
            Console.WriteLine();


            for(int x = 0; x < 64; x++)
                log_u64(bit_map_buffer[x]);
        
            floor_layout = bit_map_buffer;
        }

        private static Random random = new Random();
        private UInt64[] floor_layout = new UInt64[64];
        private readonly UInt64 map_bit_mask = 0xEFFFFFFFFFFFFFFE; // Binary: 01111111 11111111 11111111 11111111 11111111 11111111 11111111 11111110

        private void log_u64(UInt64 number) {

            string binaryRepresentation = Convert.ToString((long)number, 2).PadLeft(64, '0');

            // Replace '0' with ' ' (space) and '1' with 'X'
            char[] binaryChars = binaryRepresentation.Select(c => c == '0' ? ' ' : 'X').ToArray();
            string formattedBinary = new string(binaryChars);

            Console.WriteLine($"Map: |{formattedBinary}|");
        }


        // Function to count the number of '1's in a 3-bit number
        private static int count_ones(uint num) {

            int count = 0;
            for(int i = 0; i < 3; i++) {
                if((num & (1 << i)) != 0)
                    count++;
            }
            return count;
        }

        // Function to count the number of '1's in the leftmost and rightmost bits of a 3-bit number
        private static int count_ones_exclude_middle(uint num) {

            int count = 0;
            for(int i = 0; i <= 2; i += 2) {
                if((num & (1 << i)) != 0) 
                    count++;
            }
            return count;
        }

        // Function to generate a UInt64 value with specified density of bits flipped
        static UInt64 generate_random_UInt64_with_density(double density) {

            Random random = new Random();
            UInt64 result = 0;
            for(int i = 0; i < 64; i++) {
            
                double randomValue = random.NextDouble();
                if(randomValue < density)
                    result |= (UInt64)1 << i;
            }
            return result;
        }

    }
}


/*



*/