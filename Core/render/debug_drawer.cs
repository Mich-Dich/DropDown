
namespace Core.render {

    using Core.physics;
    using Core.render.shaders;
    using Core.util;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using System.Drawing;

    public enum DebugColor {
        
        Red,
        Green,
        Blue,
        White,
    }

    public struct debug_line {

        public Vector2 start;
        public Vector2 end;
        public double time_stamp;
        public double display_duration;

        public debug_line(Vector2 start, Vector2 end, Double time_stamp = 0, Double display_duration = 0) {

            this.start = start;
            this.end = end;
            this.time_stamp = time_stamp;
            this.display_duration = display_duration;
        }
    }

    public sealed class Debug_Drawer {

        public List<debug_line> lines = new List<debug_line>();

        private readonly Shader debugShader;
        private readonly int vbo;
        private readonly int vao;
        private readonly int ebo;

        public Debug_Drawer() {

            this.debugShader = Resource_Manager.Get_Shader("shaders/debug.vert", "shaders/debug.frag");
            this.vbo = GL.GenBuffer();
            this.vao = GL.GenVertexArray();
            this.ebo = GL.GenBuffer();
        }

        public DebugColor DebugColor { get; set; } = DebugColor.White;

        public void draw() {

            this.debugShader.Use();
            Vector4 color = new Vector4(0.0f, 1.0f, 0.0f, 0.5f);
            this.debugShader.Set_Uniform("color", color);

            Console.WriteLine($"lines: [{lines.Count}]");
            foreach(debug_line line in lines) {
                draw_line(line.start, line.end);
            }
                        
            GL.BindVertexArray(0);
            this.debugShader.Unbind();

        }

        // ================================================================= public =================================================================
        public void Draw_Collision_Shape(Transform transform, Collider collider, DebugColor debugColor) {

            // Console.WriteLine("Drawing collision shape");
            this.DebugColor = debugColor;
            this.debugShader.Use();
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

            this.debugShader.Set_Uniform("color", color);

            Transform buffer = new (transform + collider.offset);
            buffer.size = Vector2.One;
            Matrix4 matrixTransform = buffer.GetTransformationMatrix();
            Matrix4 finalTransform = matrixTransform * Game.Instance.camera.Get_Projection_Matrix();
            this.debugShader.Set_Matrix_4x4("transform", finalTransform);

            if(collider.shape == Collision_Shape.Circle)
                this.Draw_Circle((transform.size.X / 2) + (collider.offset.size.X / 2), 20);
            else if(collider.shape == Collision_Shape.Square)
                this.Draw_Rectangle(transform.size + collider.offset.size);

            GL.BindVertexArray(0);
            this.debugShader.Unbind();
        }

        public void Dispose() {

            Console.WriteLine("Disposing debug_drawer");
            this.debugShader.Dispose();

            // Delete the VBO, VAO, and EBO
            GL.DeleteBuffer(this.vbo);
            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.ebo);
        }

        // ================================================================= private =================================================================

        private void draw_line(Vector2 start, Vector2 end) {

            Console.WriteLine("Drawing Line");
            float[] vertices = {
                start.X, start.Y, 0.0f,
                end.X, end.Y, 0.0f,
            };

            uint[] indices = { 0, 1 };
            this.Draw_Shape(vertices, indices, PrimitiveType.Lines);
        }

        private void Draw_Rectangle(Vector2 size) {

            // Console.WriteLine("Drawing rectangle");
            float[] vertices = {
                -0.5f * size.X,  0.5f * size.Y, 0.0f,
                0.5f * size.X,  0.5f * size.Y, 0.0f,
                0.5f * size.X, -0.5f * size.Y, 0.0f,
                -0.5f * size.X, -0.5f * size.Y, 0.0f,
            };

            uint[] indices = { 0, 1, 2, 3 };

            this.Draw_Shape(vertices, indices, PrimitiveType.LineLoop);
        }

        private void Draw_Circle(float radius, int sides) {

            // Console.WriteLine("Drawing circle");
            List<float> vertices = new ();
            List<uint> indices = new ();

            for(int i = 0; i <= sides; i++) {
                float theta = 2.0f * MathF.PI * i / sides;
                float x = radius * MathF.Cos(theta);
                float y = radius * MathF.Sin(theta);
                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(0.0f);
                indices.Add((uint)i);
            }

            this.Draw_Shape(vertices.ToArray(), indices.ToArray(), PrimitiveType.LineLoop);
        }

        private void Draw_Shape(float[] vertices, uint[] indices, PrimitiveType primitiveType) {

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(this.vao);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int positionLocation = this.debugShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(positionLocation);

            GL.BindVertexArray(this.vao);
            GL.DrawElements(primitiveType, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}