
namespace Core.render {

    using OpenTK.Graphics.OpenGL4;

    public sealed class vertex_array : i_buffer, IDisposable {

        public int id { get; private set; }

        public vertex_array() {

            id = GL.GenVertexArray();
        }

        private bool disposed = false;
        public void Dispose() {

            if(disposed)
                GL.DeleteVertexArray(id);
            
            disposed = true;
        }

        public void add_buffer(vertex_buffer buffer, buffer_layout layout) {

            bind();
            buffer.bind();
            var elements = layout.get_buffer_elements();
            int offset = 0;

            for (int x = 0; x < elements.Count; x++) {
                
                var current_element = elements[x];
                GL.EnableVertexAttribArray(x);
                GL.VertexAttribPointer(x, current_element.count, current_element.type, current_element.normalized, layout.get_stride(), offset);
                offset += current_element.count * utility.get_size_of_VertexAttribPointerType(current_element.type);
            }
        }

        public void bind() {

            GL.BindVertexArray(id);
        }

        public void unbind() {

            GL.BindVertexArray(0);
        }

    }
}
