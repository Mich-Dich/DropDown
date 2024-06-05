namespace UnitTest
{
    using Xunit;
    using Core.util;
    using Core.render;
    using Core.render.shaders;

    public class ResourceManagerTest 
    {
        [Fact]
        public void TestGetShader()
        {
            // Arrange
            string vertexPath = "shaders/texture_vert.glsl";
            string fragmentPath = "shaders/texture_frag.glsl";

            // Act
            Shader shader1 = Resource_Manager.Get_Shader(vertexPath, fragmentPath);
            Shader shader2 = Resource_Manager.Get_Shader(vertexPath, fragmentPath);

            // Assert
            Assert.NotNull(shader1);
            Assert.NotNull(shader2);
            Assert.Same(shader1, shader2);
        }

        [Fact]
        public void TestGetTexture()
        {
            // Arrange
            string texturePath = "textures/default_grid_bright.png";

            // Act
            Texture texture1 = Resource_Manager.Get_Texture(texturePath);
            Texture texture2 = Resource_Manager.Get_Texture(texturePath);

            // Assert
            Assert.NotNull(texture1);
            Assert.NotNull(texture2);
            Assert.Same(texture1, texture2);
        }

        [Fact]
        public void TestGetSpriteBatch()
        {
            // Arrange
            string directoryPath = "textures/";

            // Act
            SpriteBatch spriteBatch1 = Resource_Manager.Get_Sprite_Batch(directoryPath);
            SpriteBatch spriteBatch2 = Resource_Manager.Get_Sprite_Batch(directoryPath);

            // Assert
            Assert.NotNull(spriteBatch1);
            Assert.NotNull(spriteBatch2);
            Assert.Same(spriteBatch1, spriteBatch2);
        }
    }
}