
namespace Core {

using Core.util;

    public sealed class SpriteBatch {

        /// <summary>
        /// Initializes a new instance of the SpriteBatch class by loading all PNG image files from the specified directory.
        /// </summary>
        /// <param name="directoryPath">The path to the directory containing the PNG image files.</param>
        /// <param name="isPixelArt">Determines whether the loaded textures should Use nearest-neighbor sampling for pixel art style rendering (default is false).</param>
        public SpriteBatch(string directoryPath, bool isPixelArt = false) {

            string[] imagePaths = Directory.GetFiles(directoryPath, "*.png");
            Array.Sort(imagePaths);

            this.frames = new List<Texture>();
            foreach(string imagePath in imagePaths) 
                this.frames.Add(Resource_Manager.Get_Texture(imagePath, isPixelArt));
            
        }

        // ================================================================= public =================================================================

        /// <summary>
        /// Gets the texture (frame) at the specified index from the batch.
        /// </summary>
        /// <param name="index">The index of the texture to retrieve.</param>
        /// <returns>The texture (frame) at the specified index.</returns>
        public Texture GetFrame(int index) { return this.frames[index]; }

        /// <summary>
        /// Gets the number of textures (frames) in the batch.
        /// </summary>
        public int FrameCount { get { return this.frames.Count; } }

        // ================================================================= private =================================================================

        private List<Texture> frames;

    }
}