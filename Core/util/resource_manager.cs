
using Core.renderer;

namespace Core.util {

    public static class resource_manager {

        private static Dictionary<string, shader> shaders = new Dictionary<string, shader>();
        private static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private static Dictionary<string, SpriteBatch> spriteBatches = new Dictionary<string, SpriteBatch>();

        //public static ResourceManager instance {
        //    get {
        //        lock(_loc) {

        //            if(_instance == null)
        //                _instance = new resource_manager();

        //            return _instance;
        //        }
        //    }
        //}

        static public shader get_shader(string vertexPath, string fragmentPath) {

            string key = vertexPath + fragmentPath;
            if(!shaders.ContainsKey(key)) 
                shaders[key] = new shader(vertexPath, fragmentPath);
            
            return shaders[key];
        }

        static public Texture GetTexture(string path, bool isPixelArt = false) {
            
            if(!textures.ContainsKey(path)) 
                textures[path] = new Texture(path, isPixelArt);
            
            return textures[path];
        }

        static public SpriteBatch GetSpriteBatch(string directoryPath, bool isPixelArt = false) {

            if(!spriteBatches.ContainsKey(directoryPath)) 
                spriteBatches[directoryPath] = new SpriteBatch(directoryPath, isPixelArt);
            
            return spriteBatches[directoryPath];
        }
    }
}