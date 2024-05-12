using Core.game_objects;
using Core.util;
using Core.visual;
using Core;
using OpenTK.Mathematics;

namespace Hell
{

    internal class Hell : Core.game
    {

        private const int TilesOnScreenWidth = 30;
        private const int TilesOnScreenHeight = 20;

        public Hell(String title, Int32 inital_window_width, Int32 inital_window_height)
            : base(title, inital_window_width, inital_window_height) { }

        private game_object test_cursor_object;

        // ========================================================= functions =========================================================

        protected override void init()
        {
            set_update_frequency(144.0f);
            show_debug_data(false);

            // ------------- init map ------------- 
            this.active_map = new base_map();


            /*
            Texture backgroundImageTexture = resource_manager.get_texture("assets/firstLevel/Background.png", true);

            game_object backgroundGameObject = new game_object(new Vector2(480, 6080));
            backgroundGameObject.set_sprite(new sprite(backgroundImageTexture));
            backgroundGameObject.set_mobility(mobility.STATIC);
            backgroundGameObject.transform.size = new Vector2(2000, 2000);
            backgroundGameObject.get_sprite().transform.size = new Vector2(2000, 2000);

            this.active_map.add_game_object(backgroundGameObject);
            */

            // ------------- init camera ------------- 
            AdjustCameraPosition();
            AdjustCameraZoom();
            this.camera.set_min_max_zoom(0.001f, 1000f);

            // ------------- init player ------------- 
            this.player_controller = new PC_default();
            this.player = new player();
            test_cursor_object = new game_object(new Vector2(150, 150));
            this.active_map.add_game_object(test_cursor_object);
        }

        protected override void shutdown() { }

        protected override void update(float delta_time)
        {

            test_cursor_object.transform.position = this.camera.convertScreenToWorldCoords(0, 0);
            Vector2 cameraPosition = this.camera.transform.position;
            cameraPosition.Y = this.player.transform.position.Y;
            this.camera.set_position(cameraPosition);
        }

        protected override void window_resize()
        {
            AdjustCameraZoom();
            AdjustCameraPosition();
        }

        protected override void render(float delta_time) { }

        protected override void render_imgui(float delta_time) { }

        //  ================================================ private ================================================ 

        private void AdjustCameraPosition()
        {
            float cameraX = this.active_map.levelWidth / 2f;
            float cameraY = this.active_map.levelHeight - (TilesOnScreenHeight * this.active_map.tileHeight / 2f);
            this.camera.set_position(new Vector2(cameraX, cameraY));
        }

        private void AdjustCameraZoom()
        {
            float zoomWidth = (float)this.window.Size.X / (TilesOnScreenWidth * this.active_map.tileWidth);
            this.camera.set_zoom(zoomWidth);
        }
    }
}