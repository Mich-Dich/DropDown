
using Core.renderer;

namespace Core.manager {

    public class ResourceManager {
        
        private Dictionary<string, shader> shaders = new Dictionary<string, shader>();
        private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private Dictionary<string, SpriteBatch> spriteBatches = new Dictionary<string, SpriteBatch>();

        public shader GetShader(string vertexPath, string fragmentPath) {

            string key = vertexPath + fragmentPath;
            if(!this.shaders.ContainsKey(key)) 
                this.shaders[key] = new shader(vertexPath, fragmentPath);
            
            return this.shaders[key];
        }

        public Texture GetTexture(string path, bool isPixelArt = false) {
            
            if(!this.textures.ContainsKey(path)) 
                this.textures[path] = new Texture(path, isPixelArt);
            
            return this.textures[path];
        }

        public SpriteBatch GetSpriteBatch(string directoryPath, bool isPixelArt = false) {

            if(!this.spriteBatches.ContainsKey(directoryPath)) 
                this.spriteBatches[directoryPath] = new SpriteBatch(directoryPath, this, isPixelArt);
            
            return this.spriteBatches[directoryPath];
        }
    }
}