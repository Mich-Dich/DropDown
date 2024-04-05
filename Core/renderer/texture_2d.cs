using OpenTK.Graphics.OpenGL4;

namespace Core.renderer {

    public class texture_2d : IDisposable {
    
        // ========================================================= public =========================================================
        public int handle { get; private set; }

        public texture_2d(int handle) {

            this.handle = handle;
        }

        ~texture_2d() {
            Dispose(false);
        }

        public void use() {

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        // ========================================================= diposing =========================================================
        public void Dispose(bool disposed) {
            
            if(!_disposed) {
                GL.DeleteTexture(handle);
                _disposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ========================================================= private =========================================================
        private bool _disposed;
    }
}
