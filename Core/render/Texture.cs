﻿
namespace Core {

    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using StbImageSharp;

    public sealed class Texture : IDisposable {

        private readonly byte[] imageData;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsPixelArt { get; set; }
        public int Handle { get; private set; }

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

                    int sourceIndex = ((y * source.Width) + x) * 4;
                    int subImageIndex = (((y - startY) * subImageWidth) + (x - startX)) * 4;
                    Array.Copy(sourceImageData, sourceIndex, this.imageData, subImageIndex, 4);
                }
            }
        }

        // ================================================================= public =================================================================
        public void Dispose() { GL.DeleteTexture(this.Handle); }

        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }

        public void Use(TextureUnit unit = TextureUnit.Texture0) {

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, this.Handle);
        }

        // ================================================================= private =================================================================
        
        private byte[] GetImageData() { return this.imageData; }

    }
}