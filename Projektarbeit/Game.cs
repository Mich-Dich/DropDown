namespace Projektarbeit
{
    using Core;
    using Projektarbeit.characters.player;
    using Projektarbeit.Levels;
    using Projektarbeit.UI;

    internal class Game : Core.Game
    {
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;

        public Game(string title, int initalWindowWidth, int initalWindowHeight)
            : base(title, initalWindowWidth, initalWindowHeight) { }

        public override void StartGame()
        {
            set_active_map(new MAP_base());
            play_state = Play_State.Playing;
            player.health = 100;
            player.IsDead = false;
            player.IsRemoved = false;
            Score = 0;
        }

        // ========================================================= functions =========================================================
        protected override void Init()
        {

            Set_Update_Frequency(144.0f);

            activeMap = new MAP_main_menu();
            player = new CH_player();
            player.IsRemoved = true;
            playerController = new PC_main(player);

            mainMenu = new MainMenu();
            mainHUD = new MainHUD();
            gameOver = new GameOver();
#if DEBUG
            Show_Performance(true);
            showDebugData(true);
            camera.Set_min_Max_Zoom(0.03f, 1.4f);
#endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) { }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime)
        {

            switch (play_state)
            {

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
    }
}