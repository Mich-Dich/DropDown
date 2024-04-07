using Core.game_objects;
using Core.renderer;
using Core.util;
using Core.visual;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DropDown {

    internal class sprite_abstraction : Core.game {

        public sprite_abstraction(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private sprite_square _floor;

        private shader _shader;

        // ========================================================= functions =========================================================

        protected override void init() {

            //window.UpdateFrequency = 144.0f;
            GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));

            _shader = new(shader.parse_shader("shaders/texture_vert.glsl", "shaders/texture_frag.glsl"), true);
            _shader.use();

            camera = new(Vector2.Zero, this.window.Size, 1);

            _floor = new sprite_square(mobility.DYNAMIC, new Vector2(100, -150), Vector2.One, new Vector2(50, 50), 0);
            _floor.add_texture("textures/floor_000.png");
        }

        protected override void shutdown() { }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            _floor.draw(_shader);
        }

    }
}
