namespace Hell
{
    using Core.UI;
    using Core.util;
    using Hell.Levels;
    using Hell.player;
    using Hell.UI;
    using ImGuiNET;

    internal class Game : Core.Game
    {
        private bool isGameOver = false;
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight)
        {
        }

        public override void Restart()
        {
            base.Restart();

            this.isGameOver = false;
            this.activeMap = new MAP_base();
            this.player = new CH_player();
            this.playerController = new PC_main(this.player);
            this.gameState = Core.GameState.Playing;
            this.Score = 0;

            this.activeMap.Add_Game_Object(this.player);
            this.activeMap.allCharacter.Add(this.player);
        }

        protected override void Init()
        {
            this.Set_Update_Frequency(144.0f);
            this.activeMap = new MAP_base();
            this.player = new CH_player();
            this.playerController = new PC_main(this.player);
            this.mainHUD = new MainHUD();
            this.gameOver = new GameOver();
            this.mainMenu = new MainMenu();
            this.gameState = Core.GameState.MainMenu;
#if DEBUG
            this.Show_Performance(true);
            this.showDebugData(true);
            this.camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
        }

        protected override void Shutdown()
        {
        }

        protected override void Update(float deltaTime)
        {
            if (this.player.health <= 0)
            {
                this.isGameOver = true;
            }
        }

        protected override void Render(float deltaTime)
        {
        }

        protected override void Render_Imgui(float deltaTime)
        {
            switch (this.gameState)
            {
                case Core.GameState.MainMenu:
                    this.mainMenu.Render();
                    break;
                case Core.GameState.Playing:
                    if (this.isGameOver)
                    {
                        this.gameOver.Render();
                    }
                    else
                    {
                        this.mainHUD.Render();
                    }

                    break;
                default:
                    break;
            }
        }
    }
}