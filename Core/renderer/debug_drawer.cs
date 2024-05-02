using System.Collections.Generic;
using Core.game_objects;
using Core.physics;
using Core.util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Core.renderer {

    public enum DebugColor {

        Red,
        Green,
        Blue,
        White,
    }

    public class debug_drawer {

        private shader debugShader;
        private int vbo, vao, ebo;

        public DebugColor DebugColor { get; set; } = DebugColor.White;

        public debug_drawer() {
            this.debugShader = resource_manager.get_shader("shaders/debug.vert", "shaders/debug.frag");
            this.vbo = GL.GenBuffer();
            this.vao = GL.GenVertexArray();
            this.ebo = GL.GenBuffer();
        }

        public void draw_collision_shape(transform transform, collider collider, DebugColor debugColor) {

            this.DebugColor = debugColor;
            this.debugShader.use();

            Vector4 color;
            switch(this.DebugColor) {
                case DebugColor.Red:
                    color = new Vector4(1.0f, 0.0f, 0.0f, 0.5f);
                    break;
                case DebugColor.Green:
                    color = new Vector4(0.0f, 1.0f, 0.0f, 0.5f);
                    break;
                case DebugColor.Blue:
                    color = new Vector4(0.0f, 0.0f, 1.0f, 0.5f);
                    break;
                default:
                    color = new Vector4(1.0f, 1.0f, 1.0f, 0.5f);
                    break;
            }

            this.debugShader.set_uniform("color", color);

            Matrix4 matrixTransform = transform.GetTransformationMatrix();
            Matrix4 finalTransform = matrixTransform * game.instance.camera.get_projection_matrix();
            this.debugShader.set_matrix_4x4("projection", finalTransform);

            if(collider.shape == collision_shape.Circle) {
                this.draw_circle(0.5f * transform.size.X, 100);
            }
            else if(collider.shape == collision_shape.Square) {
                this.draw_rectangle(transform.size);
            }

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        private void draw_rectangle(Vector2 size) {
            float[] vertices =
            {
                -0.5f * size.X,  0.5f * size.Y, 0.0f,
                0.5f * size.X,  0.5f * size.Y, 0.0f,
                0.5f * size.X, -0.5f * size.Y, 0.0f,
                -0.5f * size.X, -0.5f * size.Y, 0.0f,
            };

            uint[] indices =
            {
                0, 1, 2, 3,
            };

            this.DrawShape(vertices, indices, PrimitiveType.LineLoop);
        }

        private void draw_circle(float radius, int sides) {
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();

            for (int i = 0; i <= sides; i++)
            {
                float theta = 2.0f * MathF.PI * i / sides;
                float x = radius * MathF.Cos(theta);
                float y = radius * MathF.Sin(theta);
                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(0.0f);
                indices.Add((uint)i);
            }

            this.DrawShape(vertices.ToArray(), indices.ToArray(), PrimitiveType.LineLoop);
        }

        private void DrawShape(float[] vertices, uint[] indices, PrimitiveType primitiveType) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int positionLocation = debugShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);

            GL.BindVertexArray(vao);
            GL.DrawElements(primitiveType, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose() {
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(ebo);
            this.debugShader.Dispose();
        }
    }
}