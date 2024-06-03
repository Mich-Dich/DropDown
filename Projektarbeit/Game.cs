namespace Hell {
    using ImGuiNET;
    using Core.util;
    using Hell.player;
    using Hell.Levels;
    using Core.UI;
    using Hell.UI;

    internal class Game : Core.Game {
        private bool isGameOver = false;
        private MainHUD mainHUD;
        private GameOver gameOver;

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        // ========================================================= functions =========================================================
        protected override void Init() {
            Set_Update_Frequency(144.0f);
            this.activeMap = new MAP_base();
            this.player = new CH_player();
            this.playerController = new PC_main(player);
            mainHUD = new MainHUD();
            gameOver = new GameOver();
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) {
            if (this.player.health <= 0) {
                isGameOver = true;
            }
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {
            if (isGameOver) {
                gameOver.Render();
            } else {
                mainHUD.Render();
            }
        }
    }
}