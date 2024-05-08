
namespace Hell {

    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    internal class Hell : Core.game {
        
        private const int TilesOnScreenWidth = 30;
        private const int TilesOnScreenHeight = 20;

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private game_object test_cursor_object;

        // ========================================================= functions =========================================================

        protected override void init() {
            set_update_frequency(144.0f);
            show_debug_data(true);

            // ------------- init map ------------- 
            this.active_map = new base_map();

            // ------------- init camera ------------- 
            AdjustCameraPosition();
            AdjustCameraZoom();
            this.camera.set_min_max_zoom(0.001f, 1000f);
            
            // ------------- init player ------------- 
            this.player_controller = new PC_default();
            this.player = new CH_player();
            test_cursor_object = new game_object(new Vector2(150, 150)).set_sprite(resource_manager.get_texture("assets/defaults/default_grid_bright.png"));
            this.active_map.add_game_object(test_cursor_object);
        }
        
        protected override void shutdown() { }

        protected override void update(float delta_time) {

            test_cursor_object.transform.position = this.camera.convertScreenToWorldCoords(0, 0);
            Vector2 cameraPosition = this.camera.transform.position;
            cameraPosition.Y = this.player.transform.position.Y;
            this.camera.set_position(cameraPosition);
        }

        protected override void window_resize() {
            AdjustCameraZoom();
            AdjustCameraPosition();
        }

        protected override void render(float delta_time) { }

        protected override void render_imgui(float delta_time) { }

        //  ================================================ private ================================================ 

        private void AdjustCameraPosition() {
            float cameraX = this.active_map.levelWidth / 2f;
            float cameraY = this.active_map.levelHeight - (TilesOnScreenHeight * this.active_map.tileHeight / 2f);
            this.camera.set_position(new Vector2(cameraX, cameraY));
        }

        private void AdjustCameraZoom() {
            float zoomWidth = (float)this.window.Size.X / (TilesOnScreenWidth * this.active_map.tileWidth);
            this.camera.set_zoom(zoomWidth);
        }
    }
}