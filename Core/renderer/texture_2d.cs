using OpenTK.Graphics.OpenGL4;

namespace Core.renderer {

    public class texture_2d : IDisposable {
    
        // ========================================================= public =========================================================
        public int handle { get; private set; }
        public int width { get; set; }
        public int height { get; set; }
        public TextureUnit texture_slot { get; set; } = TextureUnit.Texture0;

        public texture_2d(int handle) {

            this.handle = handle;
        }

        public texture_2d(Int32 handle, Int32 width, Int32 height) : this(handle) {

            this.width = width;
            this.height = height;
        }

        public texture_2d(Int32 handle, Int32 width, Int32 height, TextureUnit texture_slot) : this(handle, width, height) {

            this.texture_slot = texture_slot;
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
