using OpenTK.Mathematics;
using System.Numerics;

namespace Core.game_objects {

    public class camera : game_object{

        // game_object.position     => camera position
        // game_object.scale        => camera zoom

        public camera(OpenTK.Mathematics.Vector2 position, OpenTK.Mathematics.Vector2 window_size, float zoom) {

            this.position = position;
            this.size = window_size;
            this.scale = new OpenTK.Mathematics.Vector2(zoom, zoom);
        }

        public void set_view_size(OpenTK.Mathematics.Vector2 window_size) {

            this.size = window_size;
        }

        public Matrix4 get_projection_matrix() {

            float left = this.position.X - (size.X / 2f);
            float right = this.position.X + (size.X / 2f);
            float top = this.position.Y - (size.Y / 2f);
            float bottom = this.position.Y + (size.Y / 2f);

            Matrix4 orthographic_matrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 100f);
            Matrix4 zoom_matrix = Matrix4.CreateScale(scale.X, scale.Y, 1);

            return orthographic_matrix * zoom_matrix;
        }
    }
}
