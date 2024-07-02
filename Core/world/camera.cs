
using OpenTK.Mathematics;

namespace Core.world
{
    public sealed class Camera : Game_Object
    {

        public float zoom_offset { get; set; } = 0;

        public float zoom = 0;

        public Camera(Vector2 position, Vector2 window_size, float zoom)
            : base(position, window_size, 0, Mobility.DYNAMIC)
        {

            this.transform.position = position;
            this.transform.size = window_size;
            this.scale = zoom + this.zoom_offset;
            this.Calc_Scale();
        }

        public void Set_View_Size(Vector2 window_size)
        {

            this.transform.size = window_size;
        }

        public void Set_min_Max_Zoom(float min, float max)
        {

            this.minZoom = (min >= 0.01f) ? min : 0.01f;
            this.maxZoom = max;
            this.Calc_Scale();
        }

        public void Add_Zoom(float zoom)
        {

            this.zoom += zoom;
            this.Calc_Scale();
        }

        public void Add_Zoom_Offset(float zoom_offset)
        {

            if (zoom + (this.zoom_offset + zoom_offset) >= minZoom
                && zoom + (this.zoom_offset + zoom_offset) < maxZoom)
                this.zoom_offset += zoom_offset;

            this.Calc_Scale();
        }

        public void Set_Zoom(float zoom)
        {

            if (zoom + this.zoom_offset >= minZoom
                && zoom + this.zoom_offset < maxZoom)
                this.zoom = zoom;
            this.Calc_Scale();
        }

        public void Set_Position(Vector2 position)
        {

            this.transform.position = position;
            this.Calc_Scale();
        }

        public Matrix4 Get_Projection_Matrix()
        {

            float left = this.transform.position.X - (this.transform.size.X / 2f);
            float right = this.transform.position.X + (this.transform.size.X / 2f);
            float top = this.transform.position.Y - (this.transform.size.Y / 2f);
            float bottom = this.transform.position.Y + (this.transform.size.Y / 2f);

            Matrix4 orthographic_matrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, .00f, 1000f);
            Matrix4 zoom_matrix = Matrix4.CreateScale(this.scale, this.scale, 1);

            return orthographic_matrix * zoom_matrix;
        }

        public Vector2 Get_Uper_Left_Screen_Corner_In_World_Coordinates()
        {

            Vector2 result = new();

            result = this.transform.position * this.scale;
            result -= Game.Instance.window.Size / 2;
            result /= this.scale;

            return result;
        }

        public Vector2 Get_Lower_Right_Screen_Corner_In_World_Coordinates()
        {

            Vector2 result = new();

            result = this.transform.position * this.scale;
            result += Game.Instance.window.Size / 2;
            result /= this.scale;

            return result;
        }

        public Vector2 Get_View_Size_In_World_Coord()
        {

            Vector2 camera_view_min = this.Get_Uper_Left_Screen_Corner_In_World_Coordinates();
            Vector2 camera_view_max = this.Get_Lower_Right_Screen_Corner_In_World_Coordinates();

            return new Vector2(
                camera_view_max.X - camera_view_min.X,
                camera_view_max.Y - camera_view_min.Y);
        }

        public float GetScale()
        {
            return this.scale;
        }

        public Vector2 GetFront()
        {
            return new Vector2(0, 1);
        }

        public Vector2 GetUp()
        {
            return new Vector2(1, 0);
        }

        public Matrix4 GetViewMatrix()
        {
            Vector2 cameraFront = GetFront();
            Vector2 cameraUp = GetUp();
            Vector3 cameraPosition3D = new Vector3(transform.position.X, transform.position.Y, 0);
            Vector3 cameraFront3D = new Vector3(cameraFront.X, cameraFront.Y, 0);
            Vector3 cameraUp3D = new Vector3(cameraUp.X, cameraUp.Y, 0);

            Matrix4 viewMatrix = Matrix4.LookAt(cameraPosition3D, cameraPosition3D + cameraFront3D, cameraUp3D);
            return viewMatrix;
        }

        // ========================================== private ==========================================
        private void Calc_Scale()
        {

            this.scale = Math.Clamp(this.zoom + this.zoom_offset, this.minZoom, this.maxZoom);
        }

        private float scale { get; set; }

        private float minZoom = 0.3f;
        private float maxZoom = 2.0f;
    }
}
