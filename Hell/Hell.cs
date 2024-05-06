using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    internal class Hell : Core.game {

        private const int TilesOnScreenWidth = 30;
        private const int TilesOnScreenHeight = 20;

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        // ========================================================= functions =========================================================
        protected override void init() {
            set_update_frequency(144.0f);
            show_debug_data(true);

            InitializePlayer();
            InitializeMap();
            InitializeCamera();
        }

        private void InitializePlayer() {
            this.player_controller = new PC_default();
            this.player = new player();
        }

        private void InitializeMap() {
            this.active_map = new base_map(TilesOnScreenWidth, TilesOnScreenHeight, camera);
        }

        private void InitializeCamera() {
            AdjustCameraPosition();
            AdjustCameraZoom();
            this.camera.set_min_max_zoom(0.001f, 1000f);
        }

        private void AdjustCameraPosition() {
            float cameraX = this.active_map.levelWidth / 2f;
            float cameraY = this.active_map.levelHeight - (TilesOnScreenHeight * this.active_map.tileHeight / 2f);
            this.camera.set_position(new Vector2(cameraX, cameraY));
        }

        private void AdjustCameraZoom() {
            float zoomWidth = (float)this.window.Size.X / (TilesOnScreenWidth * this.active_map.tileWidth);
            this.camera.set_zoom(zoomWidth);
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) { }

        protected override void window_resize() {
            AdjustCameraZoom();
            AdjustCameraPosition();
        }

        protected override void render(float delta_time) { }

        protected override void render_imgui(float delta_time) { }
    }
}