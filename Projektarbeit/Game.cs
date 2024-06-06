
namespace Hell {

    using Hell.Levels;
    using Hell.player;
    using Hell.UI;
    using Projektarbeit.Levels;
    using Core;

    internal class Game : Core.Game
    {
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;
        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        // ========================================================= functions =========================================================
        protected override void Init() {

            Set_Update_Frequency(144.0f);

            this.activeMap = new MAP_main_menu();
            this.player = new CH_player();
            this.player.IsRemoved = true;
            this.playerController = new PC_main(player);

            mainMenu = new MainMenu();
            mainHUD = new MainHUD();
            gameOver = new GameOver();
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {

            switch(this.play_state) {

                case Play_State.main_menu:
                    mainMenu.Render();
                    break;
                case Play_State.Playing:
                    mainHUD.Render();
                    break;
                case Play_State.dead:
                    gameOver.Render();
                    break;
            }
        }

        public override void StartGame() {
            this.set_active_map(new MAP_base());
            this.play_state = Play_State.Playing;
            this.player.health = 100;
            this.player.IsDead = false;
            this.player.IsRemoved = false;
            this.Score = 0;
        }
    }
}