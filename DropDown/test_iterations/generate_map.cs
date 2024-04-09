using Core;
using Core.game_objects;
using Core.manager;
using Core.renderer;
using Core.util;
using Core.visual;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace DropDown {

    internal class generate_map : Core.game {

        public generate_map(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private shader _shader;
        private map _map;

        // ========================================================= functions =========================================================
        protected override void init() {

            this.player_controller = new PC_default();
            set_update_frequency(144.0f);
            GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));

            _shader = new(shader.parse_shader("shaders/texture_vert.glsl", "shaders/texture_frag.glsl"), true);
            _shader.use();

            var texture_sampler_uniform_location = _shader.get_uniform_location("u_texture[0]");
            int[] samplers = new int[5] { 0, 1 ,2, 3, 4};
            GL.Uniform1(texture_sampler_uniform_location, samplers.Length, samplers);

            camera = new(Vector2.Zero, this.window.Size, 1);

            _map = new map(_shader)
                .generate_square(17, 8);
        }

        protected override void shutdown() { }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            _map.draw();
        }

    }
}
