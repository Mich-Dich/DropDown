using Core.renderer;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DropDown {

    internal class drop_down : Core.game {

        public drop_down(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private readonly float[] _vertecies = {
            // x    y     z    R   G   B   A
            -.5f,  .5f,  .0f, 1f, 0f, 0f, 1f,
             .5f,  .5f,  .0f, 0f, 1f, 0f, 1f,
             .5f, -.5f,  .0f, 0f, 0f, 1f, 1f,
            -.5f, -.5f,  .0f, 0f, 0f, 1f, 1f,
        };
        
        private int _vertex_buffer;
        private int _vertex_array;
        private shader _shader;

        protected override void init() {

            GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));

            _shader = new(shader.parse_shader("shaders/default_vert.glsl", "shaders/default_frag.glsl"), true);

            _vertex_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertex_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertecies.Length * sizeof(float), _vertecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            _vertex_array = GL.GenVertexArray();
            GL.BindVertexArray(_vertex_array);
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertex_buffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);                           // location = 0 => position
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));           // location = 1 => Color
                GL.EnableVertexAttribArray(1);
            }
            GL.BindVertexArray(0);

        }

        protected override void shutdown() {

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertex_buffer);

            GL.UseProgram(0);

        }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

            GL.Clear(ClearBufferMask.ColorBufferBit);
            _shader.use();

            GL.BindVertexArray(_vertex_buffer);

            // actual draw call
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        }

    }
}
