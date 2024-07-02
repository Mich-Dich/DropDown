using Core.render.shaders;
using Core.util;
using Core.world;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Core.render
{
    public class ParticleEffect
    {
        private readonly float[] _vertices =
        {
             // positions        // colors
             0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // bottom right
            -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // bottom left
             0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // top 
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        public Transform Transform { get; private set; }

        public ParticleEffect()
        {
            Transform = new Transform
            {
                size = new Vector2(50, 50),
                position = new Vector2(100, 200),
            };

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _shader = Resource_Manager.Get_Shader("Core.defaults.shaders.sparkle.vert", "Core.defaults.shaders.sparkle.frag");
            _shader.Use();
        }

        public void Draw()
        {
            _shader.Use();

            Matrix4 modelMatrix = Transform.GetTransformationMatrix();
            Matrix4 viewMatrix = Game.Instance.camera.GetViewMatrix();
            Matrix4 projectionMatrix = Game.Instance.camera.Get_Projection_Matrix();

            _shader.Set_Matrix_4x4("model", modelMatrix);
            _shader.Set_Matrix_4x4("view", viewMatrix);
            _shader.Set_Matrix_4x4("projection", projectionMatrix);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }
    }
}