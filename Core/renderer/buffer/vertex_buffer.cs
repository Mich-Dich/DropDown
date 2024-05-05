using OpenTK.Graphics.OpenGL4;

namespace Core.renderer {

    public class vertex_buffer : i_buffer {

        public int id { get; private set; }

        public vertex_buffer(float[] vertices) {

            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        ~vertex_buffer() {
            
            GL.DeleteBuffer(id);
        }

        public void update_content(float[] vertices) {

            // id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void bind() {

            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        }

        public void unbind() {

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
