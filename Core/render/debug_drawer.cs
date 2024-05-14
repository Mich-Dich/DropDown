
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

    public sealed class global_debug_drawer {

        public List<debug_line> lines = new List<debug_line>();

        public void draw() {

            basic_drawer.debugShader.Use();
            Vector4 color = new Vector4(0.0f, 1.0f, 0.0f, 0.5f);
            basic_drawer.debugShader.Set_Uniform("color", color);

            Matrix4 matrixTransform = new Transform().GetTransformationMatrix();
            Matrix4 finalTransform = matrixTransform * Game.Instance.camera.Get_Projection_Matrix();
            basic_drawer.debugShader.Set_Matrix_4x4("transform", finalTransform);

            Console.WriteLine($"lines: [{lines.Count}]");
            foreach(debug_line line in lines) {
                draw_line(line.start, line.end);

            }

            // clean up list
            for(int x = 0; x < lines.Count; x++) {

                if((lines[x].time_stamp + lines[x].display_duration) < Game_Time.total)
                    lines.RemoveAt(x);
            }

            GL.BindVertexArray(0);
            basic_drawer.debugShader.Unbind();
        }


        private void draw_line(Vector2 start, Vector2 end) {

            // Console.WriteLine("Drawing Line");
            float[] vertices = {
                start.X/100, start.Y/100, 0.0f,
                end.X/100, end.Y/100, 0.0f,
            };

            uint[] indices = { 0, 1 };
            basic_drawer.Draw_Shape(vertices, indices, PrimitiveType.Lines);
        }

    }

    public sealed class Debug_Drawer {

        private readonly Shader debugShader;
        public DebugColor DebugColor { get; set; } = DebugColor.White;

        public Debug_Drawer() { this.debugShader = Resource_Manager.Get_Shader("shaders/debug.vert", "shaders/debug.frag"); }

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
        }

        // ================================================================= private =================================================================

        private void Draw_Rectangle(Vector2 size) {

            // Console.WriteLine("Drawing rectangle");
            float[] vertices = {
                -0.5f * size.X,  0.5f * size.Y, 0.0f,
                0.5f * size.X,  0.5f * size.Y, 0.0f,
                0.5f * size.X, -0.5f * size.Y, 0.0f,
                -0.5f * size.X, -0.5f * size.Y, 0.0f,
            };

            uint[] indices = { 0, 1, 2, 3 };

            basic_drawer.Draw_Shape(vertices, indices, PrimitiveType.LineLoop);
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

            basic_drawer.Draw_Shape(vertices.ToArray(), indices.ToArray(), PrimitiveType.LineLoop);
        }
    }


    internal static class basic_drawer {

        public static readonly Shader debugShader = Resource_Manager.Get_Shader("shaders/debug.vert", "shaders/debug.frag");
        private static readonly int vbo = GL.GenBuffer();
        private static readonly int vao = GL.GenVertexArray();
        private static readonly int ebo = GL.GenBuffer();

        internal static void Draw_Shape(float[] vertices, uint[] indices, PrimitiveType primitiveType) {

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
    }

}