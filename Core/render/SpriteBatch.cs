
namespace Core.render {

    using Core.util;

    public sealed class SpriteBatch {
        private readonly List<Texture> frames;

        public SpriteBatch(string directoryPath, bool isPixelArt = false) {
            string[] imagePaths = Directory.GetFiles(directoryPath, "*.png");
            Array.Sort(imagePaths);

            frames = new List<Texture>();
            foreach(string imagePath in imagePaths) frames.Add(Resource_Manager.Get_Texture(imagePath, isPixelArt));
        }

        public int frameCount {
            get { return frames.Count; }
        }

        // ================================================================= public =================================================================
        public Texture GetFrame(int index) {
            return frames[index];
        }
    }
}