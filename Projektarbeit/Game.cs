
namespace Hell {

    using Hell.Levels;
    using Hell.player;
    using Hell.UI;
    using Projektarbeit.Levels;

    internal enum Play_State {

        main_menu = 0,
        Playing = 1,
        dead = 2,
    }

    internal class Game : Core.Game
    {
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;
        private Play_State play_state = Play_State.main_menu;

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        // ========================================================= functions =========================================================
        protected override void Init() {

            Set_Update_Frequency(144.0f);
            
            this.activeMap = new MAP_main_menu();
            this.player = new CH_player();
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

        protected override void Update(float deltaTime) {

            if (this.player.health <= 0)
#if true         // TEST-ONLY: Directly go to new map
                Game.Instance.set_active_map(new MAP_base());
#else
                play_state = Play_State.dead;
#endif

        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime) {

            switch(play_state) {

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

        public override void Start_Game() {

            this.activeMap = new MAP_base();
            play_state = Play_State.Playing;
        }
    }
}