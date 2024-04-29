using Core.physics;
using OpenTK.Mathematics;
using System.Numerics;

namespace Core.game_objects
{

    public class camera : game_object{

        // game_object.position     => camera position
        // game_object.scale        => camera zoom

        public float scale { get; set; }

        public camera(OpenTK.Mathematics.Vector2 position, OpenTK.Mathematics.Vector2 window_size, float zoom) {

            this.transform.position = position;
            this.transform.size = window_size;
            this.scale = zoom;
        }

        public void set_view_size(OpenTK.Mathematics.Vector2 window_size) {

            this.transform.size = window_size;
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

        public override void hit(hit_data hit) {
            throw new NotImplementedException();
        }
    }
}
