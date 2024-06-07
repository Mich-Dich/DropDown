
using OpenTK.Graphics.OpenGL4;

namespace Core.render.buffer
{
    public sealed class Index_Buffer : I_Buffer, IDisposable
    {

        public int id { get; }

        public Index_Buffer(uint[] indecies)
        {

            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indecies.Length * sizeof(uint), indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Bind() { GL.BindBuffer(BufferTarget.ElementArrayBuffer, id); }

        public void Unbind() { GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); }

        private bool disposed = false;

        public void Dispose()
        {

            if (!disposed)
                GL.DeleteBuffer(id);
            disposed = true;
        }
    }
}
