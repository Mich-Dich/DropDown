
using DropDown.utility;

namespace UnitTest.DropDown {

    using System;
    using System.Reflection;
    using Xunit;

    public class Cellular_AutomataTests {

        [Fact]
        public void Constructor_ShouldInitializeFields() {

            
            var automata = new Cellular_Automata();

            Assert.NotNull(automata.bit_map);
            Assert.Equal(64, automata.bit_map.Length);
            Assert.NotNull(automata.empty_tile_location);
            Assert.NotNull(automata.iterations);
            Assert.Equal(5, automata.iterations.Length);
            Assert.Equal(150, automata.cellSize);
            Assert.Equal(0.37f, automata.initalDensity);
        }

        [Fact]
        public void Generate_Bit_Map_ShouldGenerateValidMap() {

            var automata = new Cellular_Automata(seed: 42);
            automata.Generate_Bit_Map();

            FieldInfo bitMapGenerationIterationField = typeof(Cellular_Automata).GetField("bitMapGenerationIteration", BindingFlags.NonPublic | BindingFlags.Instance);
            int bitMapGenerationIteration = (int)bitMapGenerationIterationField.GetValue(automata);

            Assert.True(automata.empty_tile_location.Count > 0);
            Assert.True(bitMapGenerationIteration <= 1000);
        }

        [Fact]
        public void Get8x8_Block_ShouldReturnValidBlock() {

            var automata = new Cellular_Automata(seed: 42);
            automata.Generate_Bit_Map();

            var block = automata.Get8x8_Block(0, 0);

            Assert.Equal(8, block.Length);
        }

        [Fact]
        public void Get8x8_Block_InvalidCoordinates_ShouldThrowException() {

            var automata = new Cellular_Automata();
            Assert.Throws<ArgumentException>(() => automata.Get8x8_Block(-1, 0));
            Assert.Throws<ArgumentException>(() => automata.Get8x8_Block(0, -1));
            Assert.Throws<ArgumentException>(() => automata.Get8x8_Block(10, 0));
            Assert.Throws<ArgumentException>(() => automata.Get8x8_Block(0, 10));
        }

        [Fact]
        public void FindRandomFreePosition_ShouldReturnValidPosition() {

            var automata = new Cellular_Automata(seed: 42);
            automata.Generate_Bit_Map();
            var position = automata.find_random_free_positon();

            Assert.True(position.X >= -32 * automata.cellSize && position.X <= 32 * automata.cellSize);
            Assert.True(position.Y >= -32 * automata.cellSize && position.Y <= 32 * automata.cellSize);
        }


        [Fact]
        public void IsCoordInBitMapFree_ShouldReturnCorrectValue() {

            var automata = new Cellular_Automata(seed: 42);
            automata.Generate_Bit_Map();

            // Use reflection to access the private is_coord_in_bit_map_free method
            MethodInfo isCoordFreeMethod = typeof(Cellular_Automata).GetMethod("is_coord_in_bit_map_free", BindingFlags.NonPublic | BindingFlags.Instance);

            // Example coordinates for testing
            int testX = 10;
            int testY = 10;

            // Set specific bit in the bit_map for testing
            automata.bit_map[testY] = (1UL << testX); // Setting the bit at (testX, testY) to 1
            bool isFree = (bool)isCoordFreeMethod.Invoke(automata, new object[] { testX, testY });
            Assert.False(isFree);

            // Clear specific bit in the bit_map for testing
            automata.bit_map[testY] = 0; // Setting the bit at (testX, testY) to 0
            isFree = (bool)isCoordFreeMethod.Invoke(automata, new object[] { testX, testY });
            Assert.True(isFree);
        }

        [Fact]
        public void Iterate_Over_Bit_Map_ShouldProcessCorrectly() {

            var automata = new Cellular_Automata(seed: 42);
            automata.Generate_Bit_Map();

            // Take a snapshot of bit_map before iteration
            ulong[] bitMapBeforeIteration = (ulong[])automata.bit_map.Clone();

            MethodInfo Iterate_Over_Bit_Map_method = typeof(Cellular_Automata).GetMethod("Iterate_Over_Bit_Map", BindingFlags.NonPublic | BindingFlags.Instance);
            Iterate_Over_Bit_Map_method.Invoke(automata, new object[] {} );

            // Ensure bit_map is different after iteration
            bool isDifferent = false;
            for(int i = 0; i < automata.bit_map.Length; i++) {
                if(bitMapBeforeIteration[i] != automata.bit_map[i]) {
                    isDifferent = true;
                    break;
                }
            }

            Assert.True(isDifferent);
        }

        [Fact]
        public void Count_Ones_ShouldReturnCorrectCount() {

            var automata = new Cellular_Automata();
            MethodInfo Count_Ones_method = typeof(Cellular_Automata).GetMethod("Count_Ones", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.Equal(0, Count_Ones_method.Invoke(automata, new object[] { 0b000 }));
            Assert.Equal(1, Count_Ones_method.Invoke(automata, new object[] { 0b001 }));
            Assert.Equal(1, Count_Ones_method.Invoke(automata, new object[] { 0b010 }));
            Assert.Equal(2, Count_Ones_method.Invoke(automata, new object[] { 0b101 }));
            Assert.Equal(1, Count_Ones_method.Invoke(automata, new object[] { 0b111 }));
        }

        [Fact]
        public void Count_Ones_Exclude_Middle_ShouldReturnCorrectCount() {

            var automata = new Cellular_Automata();
            MethodInfo Count_Ones_Exclude_Middle_method = typeof(Cellular_Automata).GetMethod("Count_Ones_Exclude_Middle", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.Equal(0, Count_Ones_Exclude_Middle_method.Invoke(automata, new object[] { 0b000 }));
            Assert.Equal(1, Count_Ones_Exclude_Middle_method.Invoke(automata, new object[] { 0b001 }));
            Assert.Equal(1, Count_Ones_Exclude_Middle_method.Invoke(automata, new object[] { 0b100 }));
            Assert.Equal(2, Count_Ones_Exclude_Middle_method.Invoke(automata, new object[] { 0b101 }));
            Assert.Equal(0, Count_Ones_Exclude_Middle_method.Invoke(automata, new object[] { 0b010 }));
        }
    }
}
