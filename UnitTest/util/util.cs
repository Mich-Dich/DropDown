
namespace UnitTest {

    using Core.util;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Xunit;

    public class utilTest {

        [Fact]
        public void TestUtilGetSizeOfVertexAttribPointerType() {

            Assert.Equal(sizeof(byte), util.Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType.Byte));
            Assert.Equal(sizeof(short), util.Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType.UnsignedShort));
            Assert.Equal(sizeof(uint), util.Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType.UnsignedInt));
            Assert.Equal(sizeof(short), util.Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType.HalfFloat));
        }

        [Fact]
        public void TestUtilRadiansToDegree() { Assert.Equal(180, util.Radians_To_Degree(Math.PI)); }

        [Fact]
        public void TestUtilDegreeToRadians() { Assert.Equal(Math.PI, util.Degree_To_Radians(180)); }

        [Fact]
        public void TestRadiansToDegree() {

            Assert.Equal(0, util.Radians_To_Degree(0));
            Assert.Equal(180, util.Radians_To_Degree(Math.PI));
            Assert.Equal(90, util.Radians_To_Degree(Math.PI / 2));
        }

        [Fact]
        public void TestDegreeToRadians() {

            Assert.Equal(0, util.Degree_To_Radians(0));
            Assert.Equal(Math.PI, util.Degree_To_Radians(180));
            Assert.Equal(Math.PI / 2, util.Degree_To_Radians(90));
        }

        [Fact]
        public void TestLerp() {

            Assert.Equal(5, util.Lerp(0, 10, 0.5f));
            Assert.Equal(0, util.Lerp(0, 10, 0));
            Assert.Equal(10, util.Lerp(0, 10, 1));
            Assert.Equal(0, util.Lerp(0, 10, -1)); // clamped to 0
            Assert.Equal(10, util.Lerp(0, 10, 2)); // clamped to 1
        }

        [Fact]
        public void TestAngleFromOpenTKVector() {
            
            Assert.Equal(0, util.angle_from_vec(new Vector2(0, 1)), 0.0001);
            Assert.Equal(Math.PI / 2, util.angle_from_vec(new Vector2(1, 0)), 0.0001);
            Assert.Equal(-Math.PI / 2, util.angle_from_vec(new Vector2(-1, 0)), 0.0001);
            Assert.Equal(Math.PI, util.angle_from_vec(new Vector2(0, -1)), 0.0001);
        }

        [Fact]
        public void TestAngleFromBox2DXVector() {
            
            Assert.Equal(0, util.angle_from_vec(new Box2DX.Common.Vec2(0, 1)), 0.0001);
            Assert.Equal(Math.PI / 2, util.angle_from_vec(new Box2DX.Common.Vec2(1, 0)), 0.0001);
            Assert.Equal(-Math.PI / 2, util.angle_from_vec(new Box2DX.Common.Vec2(-1, 0)), 0.0001);
            Assert.Equal(Math.PI, util.angle_from_vec(new Box2DX.Common.Vec2(0, -1)), 0.0001);
        }

    }
}
