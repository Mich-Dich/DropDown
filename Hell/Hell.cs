using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    internal class Hell : Core.game {

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private float manualZoomAdjustment = 1.088f;

        // ========================================================= functions =========================================================
        protected override void init() {
            set_update_frequency(144.0f);
            show_debug_data(true);

            this.player_controller = new PC_default();
            this.player = new player();

            this.active_map = new base_map();
            Vector2 levelDimensions = new Vector2(this.active_map.levelWidth, this.active_map.levelHeight);

            float requiredZoom = inital_window_height / levelDimensions.Y;

            float scaledLevelWidth = levelDimensions.X * requiredZoom;

            if (scaledLevelWidth > inital_window_width) {
                requiredZoom = inital_window_width / levelDimensions.X;
            }

            requiredZoom *= manualZoomAdjustment;

            Console.WriteLine($"Setting zoom to: {requiredZoom} (after manual adjustment)");
            this.camera.set_zoom(requiredZoom);
            Vector2 levelCenter = new Vector2(levelDimensions.X / 2, levelDimensions.Y / 2);
            this.camera.set_position(levelCenter);
        }

        protected override void shutdown() { }

        protected override void update(float delta_time) { }

        protected override void render(float delta_time) { }

        protected override void render_imgui(float delta_time) { }
    }
}