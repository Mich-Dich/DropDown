
namespace Core {

using Core.util;

    public sealed class SpriteBatch {
        
        private List<Texture> frames;

        public SpriteBatch(string directoryPath, bool isPixelArt = false) {

            string[] imagePaths = Directory.GetFiles(directoryPath, "*.png");
            Array.Sort(imagePaths);

            this.frames = new List<Texture>();
            foreach(string imagePath in imagePaths) 
                this.frames.Add(Resource_Manager.Get_Texture(imagePath, isPixelArt));
            
        }

        public Texture GetFrame(int index) {
            
            return this.frames[index];
        }

        public int FrameCount {
            
            get { return this.frames.Count; }
        }
    }
}