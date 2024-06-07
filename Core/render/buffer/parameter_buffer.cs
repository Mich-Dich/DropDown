
using OpenTK.Graphics.OpenGL4;

namespace Core.render.buffer
{
    public sealed class Parameter_Buffer : I_Buffer, IDisposable
    {

        public int id { get; }

        public Parameter_Buffer(float[] indecies, int stride)
        {

            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
            GL.BufferData(BufferTarget.ParameterBuffer, indecies.Length * stride, indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

        private bool disposed = false;

        public void Dispose()
        {

            if (!disposed)
                GL.DeleteBuffer(id);
            disposed = true;
        }

        public void Bind() { GL.BindBuffer(BufferTarget.ParameterBuffer, id); }

        public void Unbind() { GL.BindBuffer(BufferTarget.ParameterBuffer, 0); }
    }
}
