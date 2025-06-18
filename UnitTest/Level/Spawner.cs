using Xunit;
using Core.Controllers.ai;
using OpenTK.Mathematics;
using Projektarbeit.Levels;
using System;
using System.Collections.Generic;
using Core.world;

namespace UnitTest.Projektarbeit.Levels
{
    public class SpawnerTest
    {
        [Fact]
        public void Spawner_Creation_WithValidParameters()
        {
            var position = new Vector2(100, 100);
            var controllerType = typeof(TestAIController);
            int maxSpawn = 5;
            float rate = 2.5f;
            float delay = 1.0f;
            bool active = true;

            var spawner = new Spawner(position, controllerType, maxSpawn, rate, delay, active);

            Assert.NotNull(spawner);
            Assert.True(spawner.Active);
            Assert.Equal(rate, spawner.SpawnRate);
            Assert.Equal(delay, spawner.StartDelay);
            Assert.Equal(maxSpawn, spawner.MaxSpawn);
            Assert.Equal(controllerType, spawner.ControllerType);
        }

        [Fact]
        public void Spawner_Update_WithoutSpawning()
        {
            var spawner = new Spawner(new Vector2(0, 0), typeof(TestAIController), 1, 5, 0, true);
            spawner.Update(1);

            Assert.True(spawner.Active);
        }

        [Fact]
        public void Spawner_Update_WithImmediateSpawn()
        {
            var spawner = new Spawner(new Vector2(0, 0), typeof(TestAIController), 1, 0, 0, true);
            spawner.Update(0);

            Assert.False(spawner.Active);
        }

        [Fact]
        public void Spawner_SetControllerType_WithInvalidType()
        {
            var spawner = new Spawner(new Vector2(0, 0), typeof(TestAIController), 1, 5, 0, true);

            Assert.Throws<Exception>(() => spawner.ControllerType = typeof(TestNonAIController));
        }
    }

    public class TestAIController : AI_Controller
    {
        public TestAIController(Vector2 position) : base(new List<Core.world.Character>())
        {
            // Create a test character and add it to the map
            var testCharacter = new Core.world.Character();
            characters.Add(testCharacter);
            Core.Game.Instance.get_active_map().Add_Character(testCharacter, position, 0, true);
        }
    }

    public class TestNonAIController
    {
    }
}