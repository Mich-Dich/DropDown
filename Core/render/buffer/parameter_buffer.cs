namespace Core.render
{
    using OpenTK.Graphics.OpenGL4;

    public sealed class Parameter_Buffer : I_Buffer, IDisposable
    {
        public int id { get; }

        public Parameter_Buffer(float[] indecies, int stride)
        {
            this.id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ParameterBuffer, this.id);
            GL.BufferData(BufferTarget.ParameterBuffer, indecies.Length * stride, indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

        private bool disposed = false;

        public void Dispose()
        {
            if (!this.disposed)
            {
                GL.DeleteBuffer(this.id);
            }

            this.disposed = true;
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ParameterBuffer, this.id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }
    }
}
