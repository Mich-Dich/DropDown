using Core.game_objects;
using Core.manager;
using Core.renderer;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Numerics;

namespace DropDown {

    internal class texture_test : Core.game {

        public texture_test(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private readonly float[] _verticies = {
            // x    y    UV.y  UV.x
             1f,  1f,  1f,   1f,
             1f, -1f,  1f,   0f,
            -1f, -1f,  0f,   0f,
            -1f,  1f,  0f,   1f,
        };

        private uint[] _indeices = {
            0, 1, 3,        // first triangle
            1, 2 ,3
        };

        private int _vertex_buffer;
        private int _vertex_array;
        private int _element_buffer;

        private shader _shader;
        private texture_2d _texture;

        // ========================================================= functions =========================================================

        protected override void init() { }

        protected override void load() {

            GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));

            _shader = new(shader.parse_shader("shaders/texture_vert.glsl", "shaders/texture_frag.glsl"), true);

            _vertex_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertex_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticies.Length * sizeof(float), _verticies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            _vertex_array = GL.GenVertexArray();
            GL.BindVertexArray(_vertex_array);
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertex_buffer);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);                           // location = 0 => position
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));           // location = 1 => Color
                GL.EnableVertexAttribArray(1);
            }
            //GL.BindVertexArray(0);

            _element_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _element_buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indeices.Length * sizeof(uint), _indeices, BufferUsageHint.StaticDraw);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _texture = resource_manager.instance.load_texture("textures/floor_000.png");
            _texture.use();

            camera = new(OpenTK.Mathematics.Vector2.Zero, this.window.Size, 1);
        }

        protected override void unload() {

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertex_buffer);

            GL.UseProgram(0);

        }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            System.Numerics.Vector2 position = new System.Numerics.Vector2(0, 0);
            System.Numerics.Vector2 scale = new System.Numerics.Vector2(100, 100);
            float rotation = MathF.Sin((float)delta_time.total.TotalMilliseconds) * MathF.PI * 2f;
            
            Matrix4x4 trans = Matrix4x4.CreateTranslation(position.X, position.Y, 0);
            Matrix4x4 sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
            Matrix4x4 rot = Matrix4x4.CreateRotationZ(rotation);
            _shader.set_matrix_4x4("model", sca * rot * trans);

            _shader.use();
            _shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            GL.BindVertexArray(_vertex_array);

            // actual draw call
            GL.DrawElements(PrimitiveType.Triangles, _indeices.Length, DrawElementsType.UnsignedInt, 0);

        }

    }
}
