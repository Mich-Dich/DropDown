
namespace Core.render {

    using OpenTK.Graphics.OpenGL4;

    public sealed class parameter_buffer : i_buffer, IDisposable {

        public int id { get; }

        public parameter_buffer(float[] indecies, int stride) { 
            
            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
            GL.BufferData(BufferTarget.ParameterBuffer, indecies.Length * stride, indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

        private bool _disposed = false;
        public void Dispose() {

            if(!_disposed)
                GL.DeleteBuffer(id);

            _disposed = true;
        }

        public void bind() {

            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
        }

        public void unbind() {
            
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

    }
}
