using OpenTK.Graphics.OpenGL4;
using System.Diagnostics.CodeAnalysis;

namespace Core.renderer {

    public class vertex_array : i_buffer {

        public int id { get; private set; }

        public vertex_array() {

            id = GL.GenVertexArray();
        }

        ~vertex_array() {

            if(id == 0)
                return;

            GL.DeleteVertexArray(id);
            id = 0;
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
