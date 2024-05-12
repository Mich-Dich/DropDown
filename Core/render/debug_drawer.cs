
namespace Core.render {

    using Core.physics;
    using Core.render.shaders;
    using Core.util;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public enum DebugColor {

        Red,
        Green,
        Blue,
        White,
    }

    public sealed class Debug_Drawer {
        
        private Shader debugShader;
        private int vbo, vao, ebo;

        public DebugColor DebugColor { get; set; } = DebugColor.White;

        public Debug_Drawer() {
            this.debugShader = Resource_Manager.Get_Shader("shaders/debug.vert", "shaders/debug.frag");

            this.vbo = GL.GenBuffer();
            this.vao = GL.GenVertexArray();
            this.ebo = GL.GenBuffer();
        }

        public void draw_collision_shape(Transform transform, Collider collider, DebugColor debugColor) {

            //Console.WriteLine("Drawing collision shape");
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

            Transform buffer = new Transform(transform + collider.offset);
            buffer.size = Vector2.One;
            Matrix4 matrixTransform = buffer.GetTransformationMatrix();
            Matrix4 finalTransform = matrixTransform * Game.instance.camera.Get_Projection_Matrix();
            this.debugShader.set_matrix_4x4("transform", finalTransform);

            if(collider.shape == collision_shape.Circle) {
                this.draw_circle(transform.size.X/2 + collider.offset.size.X/2, 20);
            }
            else if(collider.shape == collision_shape.Square) {
                this.draw_rectangle(transform.size + collider.offset.size);
            }

            GL.BindVertexArray(0);
            this.debugShader.unbind();
        }

        private void draw_rectangle(Vector2 size) {

            //Console.WriteLine("Drawing rectangle");
            float[] vertices = {
                -0.5f * size.X,  0.5f * size.Y, 0.0f,
                 0.5f * size.X,  0.5f * size.Y, 0.0f,
                 0.5f * size.X, -0.5f * size.Y, 0.0f,
                -0.5f * size.X, -0.5f * size.Y, 0.0f,
            };

            uint[] indices = { 0, 1, 2, 3 };

            this.DrawShape(vertices, indices, PrimitiveType.LineLoop);
        }

        private void draw_circle(float radius, int sides) {

            //Console.WriteLine("Drawing circle");
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();

            for(int i = 0; i <= sides; i++) {
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

        public void Dispose() {
            Console.WriteLine("Disposing debug_drawer");
            this.debugShader.dispose();

            // Delete the VBO, VAO, and EBO
            GL.DeleteBuffer(this.vbo);
            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.ebo);
        }
    }
}