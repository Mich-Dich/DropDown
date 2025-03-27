namespace Core.util {
    using Core.render;
    using Core.render.shaders;

    public static class Resource_Manager {
        private static readonly Dictionary<string, Shader> Shaders = new ();
        private static readonly Dictionary<string, Texture> Textures = new ();
        private static readonly Dictionary<string, SpriteBatch> SpriteBatches = new ();

        public static Shader Get_Shader(string vertexPath, string fragmentPath) {
            string key = vertexPath + fragmentPath;
            if(!Shaders.ContainsKey(key)) {
                Shaders[key] = new Shader(vertexPath, fragmentPath);
            }

            return Shaders[key];
        }

        public static Texture Get_Texture(string path, bool isPixelArt = false) {
            if(!Textures.ContainsKey(path)) {
                Textures[path] = new Texture(path, isPixelArt);
            }

            return Textures[path];
        }

        public static SpriteBatch Get_Sprite_Batch(string directoryPath, bool isPixelArt = false) {
            if(!SpriteBatches.ContainsKey(directoryPath)) {
                SpriteBatches[directoryPath] = new SpriteBatch(directoryPath, isPixelArt);
            }

            return SpriteBatches[directoryPath];
        }
    }
}