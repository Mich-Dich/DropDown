using Core.world;
using Core.util;
using Core.render;
using Core;
using OpenTK.Mathematics;

namespace Hell {

    internal class Hell : Core.Game {

        private const int TilesOnScreenWidth = 30;
        private const int TilesOnScreenHeight = 20;

        public Hell(String title, Int32 initalWindowWidth, Int32 initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        private game_object test_cursor_object;

        // ========================================================= functions =========================================================

        protected override void Init()
        {
            Set_Update_Frequency(144.0f);
            Show_DebugData(false);

            // ------------- init map ------------- 
            this.activeMap = new Base_Map();


            /*
            Texture backgroundImageTexture = resource_manager.get_texture("assets/firstLevel/Background.png", true);

            game_object backgroundGameObject = new game_object(new Vector2(480, 6080));
            backgroundGameObject.set_sprite(new sprite(backgroundImageTexture));
            backgroundGameObject.Set_Mobility(mobility.STATIC);
            backgroundGameObject.transform.size = new Vector2(2000, 2000);
            backgroundGameObject.get_sprite().transform.size = new Vector2(2000, 2000);

            this.activeMap.add_game_object(backgroundGameObject);
            */

            // ------------- init camera ------------- 
            AdjustCameraPosition();
            AdjustCameraZoom();
            this.camera.set_min_max_zoom(0.001f, 1000f);

            // ------------- init player ------------- 
            this.playerController = new PC_Default();
            this.player = new player();
            test_cursor_object = new game_object(new Vector2(150, 150));
            this.activeMap.add_game_object(test_cursor_object);
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime)
        {

            test_cursor_object.transform.position = this.camera.convertScreenToWorldCoords(0, 0);
            Vector2 cameraPosition = this.camera.transform.position;
            cameraPosition.Y = this.player.transform.position.Y;
            this.camera.set_position(cameraPosition);
        }

        protected override void Window_Resize()
        {
            AdjustCameraZoom();
            AdjustCameraPosition();
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) { }

        //  ================================================ private ================================================ 

        private void AdjustCameraPosition()
        {
            float cameraX = this.activeMap.levelWidth / 2f;
            float cameraY = this.activeMap.levelHeight - (TilesOnScreenHeight * this.activeMap.tileHeight / 2f);
            this.camera.set_position(new Vector2(cameraX, cameraY));
        }

        private void AdjustCameraZoom()
        {
            float zoomWidth = (float)this.window.Size.X / (TilesOnScreenWidth * this.activeMap.tileWidth);
            this.camera.set_zoom(zoomWidth);
        }
    }
}