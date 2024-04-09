using Core.renderer;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Core.manager.texture {

    public static class texture_manager {

        public static texture_2d load(string texture_name) {

            int handle  = GL.GenTexture();
            Enum.TryParse(typeof(TextureUnit), $"Texture{_texture_cursor}", out var parsed_enum);
            if(parsed_enum == null)
                throw new Exception($"Exceeded maximum texture slots OpenGL can nativly support. Count: [{_texture_cursor}]");

            TextureUnit texture_unit = (TextureUnit)parsed_enum;
            GL.ActiveTexture(texture_unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            FileStream stream = File.OpenRead(texture_name);
            ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            ImageInfo? info = ImageInfo.FromStream(stream);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, info.Value.Width, info.Value.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, result.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);               // REPEAT on X axis
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);               // REPEAT on Y axis
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            _texture_cursor++;
            return new texture_2d(handle, info.Value.Width, info.Value.Height, texture_unit);
        }
     
        private static int _texture_cursor = 0;

    }
}
