using Core.physics;
using OpenTK.Mathematics;
using System.Drawing;

namespace Core.game_objects
{

    public class camera : game_object{

        public float    zoom_offset { get; set; } = 0;
        public float    zoom = 0;

        public camera(OpenTK.Mathematics.Vector2 position, OpenTK.Mathematics.Vector2 window_size, float zoom)
            :base(position, window_size, 0, mobility.DYNAMIC) {

            this.transform.position = position;
            this.transform.size = window_size;
            this.scale = zoom + zoom_offset;
            calc_scale();
        }

        public void set_view_size(OpenTK.Mathematics.Vector2 window_size) {

            this.transform.size = window_size;
        }

        public void set_min_max_zoom(float min, float max) {

            this.min_zoom = (min >= 0.01f) ? min : 0.01f;
            this.max_zoom = max;
            calc_scale();
        }

        public void add_zoom(float zoom) {

            this.zoom += zoom;
            calc_scale();
        }

        public void add_zoom_offset(float zoom_offset) {

            this.zoom_offset += zoom_offset;
            calc_scale();
        }

        public void set_zoom(float zoom) {

            this.zoom = zoom;
            calc_scale();
        }

        public void set_position(Vector2 position) {

            this.transform.position = position;
            calc_scale();
        }

        public Matrix4 get_projection_matrix() {

            float left = this.transform.position.X - (transform.size.X / 2f);
            float right = this.transform.position.X + (transform.size.X / 2f);
            float top = this.transform.position.Y - (transform.size.Y / 2f);
            float bottom = this.transform.position.Y + (transform.size.Y / 2f);

            Matrix4 orthographic_matrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 1000f);
            Matrix4 zoom_matrix = Matrix4.CreateScale(scale, scale, 1);

            return orthographic_matrix * zoom_matrix;
        }

        public Vector2 convertScreenToWorldCoords(float x, float y) {

            Vector2 result = new Vector2();
            Vector2 default_offset = new Vector2(17, 40);
            Vector2 mouse_position = new Vector2(x, y);

            result = this.transform.position * this.scale;
            result += mouse_position - ((game.instance.window.Size / 2));
            result += (default_offset * ((mouse_position / game.instance.window.Size)));
            result /= this.scale;

            return result;
        }

        public Vector2 get_uper_left_screen_corner_in_world_coordinates() {

            Vector2 result = new Vector2();

            result = this.transform.position * this.scale;
            result -= ((game.instance.window.Size / 2));
            result /= this.scale;

            return result;
        }

        public Vector2 get_lower_right_screen_corner_in_world_coordinates() {

            Vector2 result = new Vector2();

            result = this.transform.position * this.scale;
            result += (game.instance.window.Size / 2);
            result /= this.scale;

            return result;
        }

        public Vector2 get_view_size_in_world_coord() {

            Vector2 camera_view_min = this.get_uper_left_screen_corner_in_world_coordinates();
            Vector2 camera_view_max = this.get_lower_right_screen_corner_in_world_coordinates();

            return new Vector2(
                camera_view_max.X - camera_view_min.X,
                camera_view_max.Y - camera_view_min.Y);
        }

        // ========================================== private ========================================== 

        private void calc_scale() {

            this.scale = Math.Clamp(this.zoom + this.zoom_offset, min_zoom, max_zoom);
        }

        private float scale { get; set; }    // camera zoom
        private float min_zoom = 0.3f;
        private float max_zoom = 2.0f;
    }
}
