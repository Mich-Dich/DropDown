namespace UnitTest
{
    using Xunit;
    using System.Numerics;
    using Core.util;
    using OpenTK.Graphics.OpenGL4;


    public class utilTest 
    {
        [Fact]
        public void TestUtilGetSizeOfVertexAttribPointerType()
        {
            Assert.Equal(sizeof(byte), util.Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType.Byte));
            Assert.Equal(sizeof(int), util.Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType.Int));
                // Add more assertions for other VertexAttribPointerType values
        }

        [Fact]
        public void TestUtilRadiansToDegree()
        {
            Assert.Equal(180, util.Radians_To_Degree(Math.PI));
        }

        [Fact]
        public void TestUtilDegreeToRadians()
        {
            Assert.Equal(Math.PI, util.Degree_To_Radians(180));
        }
    }
}