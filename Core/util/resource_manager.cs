
namespace Core.util {

    using Core.render.shaders;

    public static class Resource_Manager {

        private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        private static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private static Dictionary<string, SpriteBatch> spriteBatches = new Dictionary<string, SpriteBatch>();

        /// <summary>
        /// Retrieves or creates a shader based on the specified vertex and fragment shader file paths.
        /// </summary>
        /// <param name="vertexPath">The path to the vertex shader file.</param>
        /// <param name="fragmentPath">The path to the fragment shader file.</param>
        /// <returns>The shader associated with the provided file paths.</returns>
        static public Shader Get_Shader(string vertexPath, string fragmentPath) {

            string key = vertexPath + fragmentPath;
            if(!shaders.ContainsKey(key)) 
                shaders[key] = new Shader(vertexPath, fragmentPath);
            return shaders[key];
        }

        /// <summary>
        /// Retrieves or creates a texture based on the specified image file path.
        /// </summary>
        /// <param name="path">The path to the texture image file.</param>
        /// <param name="isPixelArt">Flag indicating whether the texture should be treated as pixel art.</param>
        /// <returns>The texture associated with the provided image file path.</returns>
        static public Texture Get_Texture(string path, bool isPixelArt = false) {
            
            if(!textures.ContainsKey(path)) 
                textures[path] = new Texture(path, isPixelArt);
            return textures[path];
        }

        /// <summary>
        /// Retrieves or creates a sprite batch for the specified directory path.
        /// </summary>
        /// <param name="directoryPath">The directory path containing sprite images.</param>
        /// <param name="isPixelArt">Flag indicating whether the sprites in the batch should be treated as pixel art.</param>
        /// <returns>The sprite batch associated with the provided directory path.</returns>
        static public SpriteBatch Get_Sprite_Batch(string directoryPath, bool isPixelArt = false) {

            if(!spriteBatches.ContainsKey(directoryPath)) 
                spriteBatches[directoryPath] = new SpriteBatch(directoryPath, isPixelArt);
            return spriteBatches[directoryPath];
        }
    }
}