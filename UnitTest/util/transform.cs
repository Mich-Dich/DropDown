namespace UnitTest
{
    using Xunit;
    using System.Numerics;
    using Core.util;
    using Core.world;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public class transformTest
    {
        [Fact]
        public void TestTransformPosition()
        {
            var transform = new Transform(new OpenTK.Mathematics.Vector2(10, 10));
            Assert.Equal(new OpenTK.Mathematics.Vector2(10, 10), transform.position);
            transform.position = new OpenTK.Mathematics.Vector2(20, 20);
            Assert.Equal(new OpenTK.Mathematics.Vector2(20, 20), transform.position);
        }

        [Fact]
        public void TestTransformAddition()
        {
            var t1 = new Transform(new OpenTK.Mathematics.Vector2(10, 10), new OpenTK.Mathematics.Vector2(5, 5), 45, Mobility.DYNAMIC);
            var t2 = new Transform(new OpenTK.Mathematics.Vector2(20, 20), new OpenTK.Mathematics.Vector2(10, 10), 90, Mobility.STATIC);
            var t3 = t1 + t2;
            Assert.Equal(new OpenTK.Mathematics.Vector2(30, 30), t3.position);
            Assert.Equal(new OpenTK.Mathematics.Vector2(15, 15), t3.size);
            Assert.Equal(135, t3.rotation);
            Assert.Equal(Mobility.DYNAMIC, t3.mobility);
            Assert.Null(t3.parent);
        }
    }
}