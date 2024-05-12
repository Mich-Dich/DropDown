﻿
namespace Core.render {

    using OpenTK.Graphics.OpenGL4;

    public sealed class Vertex_Buffer : I_Buffer, IDisposable {

        public int id { get; private set; }

        public Vertex_Buffer(float[] vertices) {

            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private bool _disposed = false;
        public void Dispose() {

            if(!_disposed) 
                GL.DeleteBuffer(id);
            
            _disposed = true;
        }

        public void update_content(float[] vertices) {

            // id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind() {

            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        }

        public void Unbind() {

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

    }
}
