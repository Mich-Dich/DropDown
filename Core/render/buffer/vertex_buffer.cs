namespace Core.render
{
    using OpenTK.Graphics.OpenGL4;

    public sealed class Vertex_Buffer : I_Buffer, IDisposable
    {
        public int id { get; private set; }

        public Vertex_Buffer(float[] vertices)
        {
            this.id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.id);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
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

        public void Update_content(float[] vertices)
        {
            // id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.id);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
