using Xunit;
using Core.Controllers.ai;
using OpenTK.Mathematics;
using Projektarbeit.Levels;
using System;

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
            bool active = true;
            float rate = 2.5f;
            float delay = 1.0f;

            var spawner = new Spawner(position, controllerType, maxSpawn, active, rate, delay);

            Assert.NotNull(spawner);
            Assert.True(spawner.Active);
            Assert.Equal(rate, spawner.SpawnRate);
            Assert.Equal(delay, spawner.Delay);
            Assert.Equal(maxSpawn, spawner.MaxSpawn);
            Assert.Equal(controllerType, spawner.ControllerType);
        }

        [Fact]
        public void Spawner_Update_WithoutSpawning()
        {
            var spawner = new Spawner(new Vector2(0, 0), typeof(TestAIController), 1, true, 5, 0);
            spawner.Update(1);

            Assert.True(spawner.Active);
        }

        [Fact]
        public void Spawner_Update_WithImmediateSpawn()
        {
            var spawner = new Spawner(new Vector2(0, 0), typeof(TestAIController), 1, true, 0, 0);
            spawner.Update(0);

            Assert.False(spawner.Active);
        }

        [Fact]
        public void Spawner_SetControllerType_WithInvalidType()
        {
            var spawner = new Spawner(new Vector2(0, 0), typeof(TestAIController), 1, true, 5, 0);

            Assert.Throws<Exception>(() => spawner.ControllerType = typeof(TestNonAIController));
        }
    }

    public class TestAIController : AI_Controller
    {
        public TestAIController(Vector2 position) : base(position)
        {
        }
    }

    public class TestNonAIController
    {
    }
}