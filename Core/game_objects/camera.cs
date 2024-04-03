using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.game_objects {

    public class camera : game_object{

        // game_object.position     => camera position
        // game_object.scale        => camera zoom

        public camera(OpenTK.Mathematics.Vector2 position, OpenTK.Mathematics.Vector2 window_size, float zoom) {

            this.position = position;
            this.size = window_size;
            this.scale = zoom;
        }

        public void set_view_size(OpenTK.Mathematics.Vector2 window_size) {

            this.size = window_size;
        }

        public Matrix4x4 get_projection_matrix() {

            float left = this.position.X - (size.X / 2f);
            float right = this.position.X + (size.X / 2f);
            float top = this.position.Y - (size.Y / 2f);
            float bottom = this.position.Y + (size.Y / 2f);

            Matrix4x4 orthographic_matrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 100f);
            Matrix4x4 zoom_matrix = Matrix4x4.CreateScale(scale);

            return orthographic_matrix * zoom_matrix;
        }
    }
}
