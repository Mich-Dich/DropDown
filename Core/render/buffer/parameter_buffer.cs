
namespace Core.render {

    using OpenTK.Graphics.OpenGL4;

    public sealed class Parameter_Buffer : I_Buffer, IDisposable {

        public int id { get; }

        public Parameter_Buffer(float[] indecies, int stride) { 
            
            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
            GL.BufferData(BufferTarget.ParameterBuffer, indecies.Length * stride, indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

        private bool _Disposed = false;
        public void Dispose() {

            if(!_Disposed)
                GL.DeleteBuffer(id);

            _Disposed = true;
        }

        public void Bind() {

            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
        }

        public void Unbind() {
            
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

    }
}
