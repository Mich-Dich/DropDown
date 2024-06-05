
namespace Hell {

    using Hell.Levels;
    using Hell.player;
    using Hell.UI;
    using Projektarbeit.Levels;

    internal class Game : Core.Game
    {
        private bool isGameOver = false;
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        // ========================================================= functions =========================================================
        protected override void Init() {

            Set_Update_Frequency(144.0f);
            //this.player = new CH_player();
            this.activeMap = new MAP_main_menu();
            //this.playerController = new PC_main(player);
            mainMenu = new MainMenu();
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

            if (this.get_active_map().GetType() == typeof(MAP_main_menu))
                mainMenu.Render();

            if(isGameOver && this.activeMap.GetType() == typeof(MAP_base) && gameOver != null)
                gameOver.Render();
            else if(this.activeMap.GetType() == typeof(MAP_base) && mainHUD != null)
                mainHUD.Render();
        }

        public override void Start_Game() {

            this.activeMap = new MAP_base();
            this.player = new CH_player();
            this.playerController = new PC_main(player);
            mainHUD = new MainHUD();
            isGameOver = false;
        }
    }
}