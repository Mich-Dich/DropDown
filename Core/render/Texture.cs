
namespace Core {

    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using StbImageSharp;

    public sealed class Texture : IDisposable {

        private byte[] imageData;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsPixelArt { get; set; }
        public int Handle { get; private set; }

        /// <summary>
        /// Constructs a new texture from the specified image file.
        /// </summary>
        /// <param name="path">The path to the image file.</param>
        /// <param name="isPixelArt">Determines whether the texture should Use nearest-neighbor sampling for pixel art style rendering (default is false).</param>
        /// <exception cref="FileNotFoundException">Thrown if the specified image file is not found.</exception>
        public Texture(string path, bool isPixelArt = false) {

            if(!File.Exists(path)) {
                string workingDirectory = Directory.GetCurrentDirectory();
                throw new FileNotFoundException($"The file at path {path} could not be found. Current working directory is {workingDirectory}.");
            }

            this.IsPixelArt = isPixelArt;
            this.Handle = GL.GenTexture();

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            GL.BindTexture(TextureTarget.Texture2D, this.Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            if(this.IsPixelArt) {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }
            else {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            this.Width = image.Width;
            this.Height = image.Height;
            this.imageData = image.Data;
        }

        /// <summary>
        /// Constructs a sub-image texture from a source texture, specifying a region by normalized coordinates.
        /// </summary>
        /// <param name="source">The source texture from which to extract a sub-image.</param>
        /// <param name="min">The minimum normalized coordinates (0 to 1) defining the top-left corner of the sub-image.</param>
        /// <param name="max">The maximum normalized coordinates (0 to 1) defining the bottom-right corner of the sub-image.</param>
        public Texture(Texture source, Vector2 min, Vector2 max) {
            
            this.Width = (int)((max.X - min.X) * source.Width);
            this.Height = (int)((max.Y - min.Y) * source.Height);
            this.IsPixelArt = source.IsPixelArt;
            this.Handle = source.Handle;

            int startX = (int)(min.X * source.Width);
            int startY = (int)(min.Y * source.Height);
            int endX = (int)(max.X * source.Width);
            int endY = (int)(max.Y * source.Height);
            int subImageWidth = endX - startX;
            int subImageHeight = endY - startY;
            this.imageData = new byte[subImageWidth * subImageHeight * 4];

            byte[] sourceImageData = source.GetImageData();
            for(int y = startY; y < endY; y++) {
                for(int x = startX; x < endX; x++) {
                    int sourceIndex = (y * source.Width + x) * 4;
                    int subImageIndex = ((y - startY) * subImageWidth + (x - startX)) * 4;
                    Array.Copy(sourceImageData, sourceIndex, this.imageData, subImageIndex, 4);
                }
            }
        }

        // ================================================================= public =================================================================

        /// <summary>
        /// Releases the OpenGL texture resource associated with this object.
        /// </summary>
        public void Dispose() { GL.DeleteTexture(this.Handle); }

        /// <summary>
        /// Unbinds the currently bound texture from the active texture unit.
        /// </summary>
        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }

        /// <summary>
        /// Binds this texture to the specified texture unit for Use in rendering.
        /// </summary>
        /// <param name="unit">Texture unit to bind this texture to (default is TextureUnit.Texture0).</param>
        public void Use(TextureUnit unit = TextureUnit.Texture0) {
        
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, this.Handle);
        }

        // ================================================================= private =================================================================

        private byte[] GetImageData() { return this.imageData; }
    }
}