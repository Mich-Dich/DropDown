using Core.game_objects;
using Core.manager;
using Core.renderer;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Numerics;

namespace DropDown {

    internal class buffer_abstraction : Core.game {

        public buffer_abstraction(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private readonly float[] _verticies = {
        //   x    y    UV.y  UV.x
             1f,  1f,  1f,   1f,
             1f, -1f,  1f,   0f,
            -1f, -1f,  0f,   0f,
            -1f,  1f,  0f,   1f,
        };

        private uint[] _indeices = {
            0, 1, 3,
            1, 2 ,3
        };

        private index_buffer _index_buffer;
        private vertex_buffer _vertex_buffer;
        private vertex_array _vertex_array;

        private shader _shader;
        private texture_2d _texture;

        // ========================================================= functions =========================================================

        protected override void init() { }

        protected override void load() {

            GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));

            _shader = new(shader.parse_shader("shaders/texture_vert.glsl", "shaders/texture_frag.glsl"), true);

            _vertex_buffer = new vertex_buffer(_verticies);
            _vertex_buffer.bind();

            buffer_layout layout = new buffer_layout();
            layout.add<float>(2);
            layout.add<float>(2);
            
            _vertex_array = new();
            _vertex_array.add_buffer(_vertex_buffer, layout);

            _index_buffer = new index_buffer(_indeices);

            _texture = resource_manager.instance.load_texture("textures/floor_000.png");
            _texture.use();

            camera = new(OpenTK.Mathematics.Vector2.Zero, this.window.Size, 1);
        }

        protected override void unload() {

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);
        }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            System.Numerics.Vector2 position = new System.Numerics.Vector2(0, 0);
            System.Numerics.Vector2 scale = new System.Numerics.Vector2(50, 50);
            float rotation = 0; /*MathF.Sin((float)delta_time.total.TotalMilliseconds) * MathF.PI * 2f;*/

            Matrix4x4 trans = Matrix4x4.CreateTranslation(position.X, position.Y, 0);
            Matrix4x4 sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
            Matrix4x4 rot = Matrix4x4.CreateRotationZ(rotation);

            _shader.use();
            _shader.set_matrix_4x4("model", sca * rot * trans);
            _shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            _vertex_array.bind();
            _index_buffer.bind();

            // actual draw call
            GL.DrawElements(PrimitiveType.Triangles, _indeices.Length, DrawElementsType.UnsignedInt, 0);
        }

    }
}
