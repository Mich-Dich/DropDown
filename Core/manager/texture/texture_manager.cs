using Core.renderer;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Core.manager.texture {

    public static class texture_manager {

        public static texture_2d load(string texture_name) {

            int handle  = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            using var image = new Bitmap(texture_name);

            // Warning origen, currently unknown
            // downgrading to an older version (5.0.0) of [System.Drawing.Common] can hide the warning but not solve root problem
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            var data = image.LockBits(
                new Rectangle(0,0,image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);               // REPEAT on X axis
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);               // REPEAT on Y axis

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new texture_2d(handle);
        }

    }
}
