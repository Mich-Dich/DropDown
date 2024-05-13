namespace Core.render
{
    using OpenTK.Graphics.OpenGL4;

    public sealed class Vertex_Array : I_Buffer, IDisposable
    {
        public int id { get; private set; }

        public Vertex_Array()
        {
            this.id = GL.GenVertexArray();
        }

        private bool disposed = false;

        public void Dispose()
        {
            if (this.disposed)
            {
                GL.DeleteVertexArray(this.id);
            }

            this.disposed = true;
        }

        public void Add_Buffer(Vertex_Buffer buffer, Buffer_Layout layout)
        {
            this.Bind();
            buffer.Bind();
            var elements = layout.get_buffer_elements();
            int offset = 0;

            for (int x = 0; x < elements.Count; x++)
            {
                var current_element = elements[x];
                GL.EnableVertexAttribArray(x);
                GL.VertexAttribPointer(x, current_element.count, current_element.type, current_element.normalized, layout.get_stride(), offset);
                offset += current_element.count * Util.Get_Size_Of_VertexAttribPointerType(current_element.type);
            }
        }

        public void Bind()
        {
            GL.BindVertexArray(this.id);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}
