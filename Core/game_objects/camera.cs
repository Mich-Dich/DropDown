using Core.physics;
using OpenTK.Mathematics;

namespace Core.game_objects
{

    public class camera : game_object{

        public float scale { get; set; }    // camera zoom

        public camera(OpenTK.Mathematics.Vector2 position, OpenTK.Mathematics.Vector2 window_size, float zoom)
            :base(position, window_size, 0, mobility.DYNAMIC) {

            this.transform.position = position;
            this.transform.size = window_size;
            this.scale = zoom;
        }

        public void set_view_size(OpenTK.Mathematics.Vector2 window_size) {

            this.transform.size = window_size;
        }

        public void set_min_max_zoom(float min, float max) {

            this.min_zoom = min;
            this.max_zoom = max;
        }

        public void add_zoom(float zoom) {

            this.scale += zoom;
            this.scale = Math.Clamp(this.scale, min_zoom, max_zoom);
        }

        public void set_zoom(float zoom) {

            this.scale = zoom;
            this.scale = Math.Clamp(this.scale, min_zoom, max_zoom);
        }

        public void set_position(Vector2 position) {

            this.transform.position = position;
        }

        public Matrix4 get_projection_matrix() {

            float left = this.transform.position.X - (transform.size.X / 2f);
            float right = this.transform.position.X + (transform.size.X / 2f);
            float top = this.transform.position.Y - (transform.size.Y / 2f);
            float bottom = this.transform.position.Y + (transform.size.Y / 2f);

            Matrix4 orthographic_matrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 100f);
            Matrix4 zoom_matrix = Matrix4.CreateScale(scale, scale, 1);

            return orthographic_matrix * zoom_matrix;
        }

        // ========================================== private ========================================== 

        private float min_zoom = 0.3f;
        private float max_zoom = 2.0f;
    }
}
