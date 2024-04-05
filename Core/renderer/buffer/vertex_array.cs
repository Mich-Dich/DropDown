using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Core.renderer {

    public class vertex_array : i_buffer {

        public int id { get; }

        public vertex_array() {

            id = GL.GenVertexArray();
        }

        ~vertex_array() {

            GL.DeleteVertexArray(id);
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
