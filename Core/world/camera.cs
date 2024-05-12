
namespace Core.world {

    using OpenTK.Mathematics;

    public sealed class Camera : Game_Object{

        /// <summary> The zoom offset applied to the camera zoom </summary>
        public float    zoom_offset { get; set; } = 0;
        /// <summary> The current zoom of the camera </summary>
        public float    zoom = 0;

        /// <summary>
        /// Constructs a new Camera instance with the specified position, window size, and zoom level.
        /// </summary>
        /// <param name="position">The initial position of the camera.</param>
        /// <param name="window_size">The size of the camera's viewport.</param>
        /// <param name="zoom">The initial zoom level of the camera.</param>
        public Camera(Vector2 position, Vector2 window_size, float zoom)
            :base(position, window_size, 0, Mobility.DYNAMIC) {

            this.transform.position = position;
            this.transform.size = window_size;
            this.scale = zoom + zoom_offset;
            Calc_Scale();
        }

        /// <summary>
        /// Sets the size of the camera's viewport.
        /// </summary>
        /// <param name="window_size">The new size of the viewport.</param>
        public void Set_View_Size(Vector2 window_size) {

            this.transform.size = window_size;
        }

        /// <summary>
        /// Sets the minimum and maximum zoom levels for the camera.
        /// </summary>
        /// <param name="min">The minimum allowable zoom level.</param>
        /// <param name="max">The maximum allowable zoom level.</param>
        public void Set_min_Max_Zoom(float min, float max) {

            this.min_zoom = (min >= 0.01f) ? min : 0.01f;
            this.max_zoom = max;
            Calc_Scale();
        }

        /// <summary>
        /// Adjusts the current zoom level of the camera.
        /// </summary>
        /// <param name="zoom">The amount to add to the current zoom level.</param>
        public void Add_Zoom(float zoom) {

            this.zoom += zoom;
            Calc_Scale();
        }

        /// <summary>
        /// Adjusts the zoom offset applied to the camera.
        /// </summary>
        /// <param name="zoom_offset">The amount to add to the zoom offset.</param>
        public void Add_Zoom_Offset(float zoom_offset) {

            this.zoom_offset += zoom_offset;
            Calc_Scale();
        }

        /// <summary>
        /// Sets the zoom level of the camera.
        /// </summary>
        /// <param name="zoom">The new zoom level to set.</param>
        public void Set_Zoom(float zoom) {

            this.zoom = zoom;
            Calc_Scale();
        }

        /// <summary>
        /// Sets the position of the camera.
        /// </summary>
        /// <param name="position">The new position of the camera.</param>
        public void Set_Position(Vector2 position) {

            this.transform.position = position;
            Calc_Scale();
        }

        /// <summary>
        /// Calculates and returns the projection matrix for the camera.
        /// </summary>
        /// <returns>The projection matrix based on the camera's properties.</returns>
        public Matrix4 Get_Projection_Matrix() {

            float left = this.transform.position.X - (transform.size.X / 2f);
            float right = this.transform.position.X + (transform.size.X / 2f);
            float top = this.transform.position.Y - (transform.size.Y / 2f);
            float bottom = this.transform.position.Y + (transform.size.Y / 2f);

            Matrix4 orthographic_matrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 1000f);
            Matrix4 zoom_matrix = Matrix4.CreateScale(scale, scale, 1);

            return orthographic_matrix * zoom_matrix;
        }

        /// <summary>
        /// Converts screen coordinates to world coordinates based on the camera's properties.
        /// </summary>
        /// <param name="x">The x-coordinate in screen space.</param>
        /// <param name="y">The y-coordinate in screen space.</param>
        /// <returns>The corresponding world coordinates.</returns>
        public Vector2 Convert_Screen_To_World_Coords(float x, float y) {

            Vector2 result = new Vector2();
            Vector2 default_offset = new Vector2(17, 40);
            Vector2 mouse_position = new Vector2(x, y);

            result = this.transform.position * this.scale;
            result += mouse_position - ((Game.instance.window.Size / 2));
            result += (default_offset * ((mouse_position / Game.instance.window.Size)));
            result /= this.scale;

            return result;
        }

        /// <summary>
        /// Gets the upper-left screen corner position in world coordinates.
        /// </summary>
        /// <returns>The upper-left corner position in world coordinates.</returns>
        public Vector2 Get_Uper_Left_Screen_Corner_In_World_Coordinates() {

            Vector2 result = new Vector2();

            result = this.transform.position * this.scale;
            result -= ((Game.instance.window.Size / 2));
            result /= this.scale;

            return result;
        }

        /// <summary>
        /// Gets the lower-right screen corner position in world coordinates.
        /// </summary>
        /// <returns>The lower-right corner position in world coordinates.</returns>
        public Vector2 Get_Lower_Right_Screen_Corner_In_World_Coordinates() {

            Vector2 result = new Vector2();

            result = this.transform.position * this.scale;
            result += (Game.instance.window.Size / 2);
            result /= this.scale;

            return result;
        }

        /// <summary>
        /// Gets the size of the camera's view in world coordinates.
        /// </summary>
        /// <returns>The size of the camera's view in world coordinates.</returns>
        public Vector2 Get_View_Size_In_World_Coord() {

            Vector2 camera_view_min = this.Get_Uper_Left_Screen_Corner_In_World_Coordinates();
            Vector2 camera_view_max = this.Get_Lower_Right_Screen_Corner_In_World_Coordinates();

            return new Vector2(
                camera_view_max.X - camera_view_min.X,
                camera_view_max.Y - camera_view_min.Y);
        }

        // ========================================== private ========================================== 

        private void Calc_Scale() {

            this.scale = Math.Clamp(this.zoom + this.zoom_offset, min_zoom, max_zoom);
        }

        private float scale { get; set; }
        private float min_zoom = 0.3f;
        private float max_zoom = 2.0f;
    }
}
