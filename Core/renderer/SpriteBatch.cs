


using Core.manager;

namespace Core {
    
    public class SpriteBatch {
        
        private List<Texture> frames;

        public SpriteBatch(string directoryPath, ResourceManager resourceManager, bool isPixelArt = false) {

            string[] imagePaths = Directory.GetFiles(directoryPath, "*.png");
            Array.Sort(imagePaths);

            this.frames = new List<Texture>();
            foreach(string imagePath in imagePaths) {
                this.frames.Add(resourceManager.GetTexture(imagePath, isPixelArt));
            }
        }

        public Texture GetFrame(int index) {
            
            return this.frames[index];
        }

        public int FrameCount {
            
            get { return this.frames.Count; }
        }
    }
}