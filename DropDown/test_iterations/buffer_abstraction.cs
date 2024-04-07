﻿using Core.game_objects;
using Core.manager;
using Core.renderer;
using Core.util;
using Core.visual;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Numerics;

namespace DropDown {

    internal class buffer_abstraction : Core.game {

        public buffer_abstraction(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private sprite_square _floor;

        private shader _shader;

        // ========================================================= functions =========================================================

        protected override void init() {

            //window.UpdateFrequency = 144.0f;
            GL.ClearColor(new Color4(.2f, .2f, .2f, 1f));

            _shader = new(shader.parse_shader("shaders/texture_vert.glsl", "shaders/texture_frag.glsl"), true);

            _floor = new sprite_square(mobility.DYNAMIC, OpenTK.Mathematics.Vector2.Zero, OpenTK.Mathematics.Vector2.One, OpenTK.Mathematics.Vector2.One, 0);
            _floor.add_texture("textures/floor_000.png");

            camera = new(OpenTK.Mathematics.Vector2.Zero, this.window.Size, 1);
        }

        protected override void shutdown() {

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);
        }

        protected override void update(game_time delta_time) { }

        protected override void render(game_time delta_time) {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.use();
            _shader.set_matrix_4x4("projection", camera.get_projection_matrix());

            // actual draw call
            _floor.draw(_shader);
        }

    }
}
