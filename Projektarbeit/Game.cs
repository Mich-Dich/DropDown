namespace Projektarbeit
{
    using Core;
    using Projektarbeit.characters.player;
    using Projektarbeit.Levels;
    using Projektarbeit.UI;
    using Core.world;

    internal class Game : Core.Game
    {
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;
        private SkillTreeMenu skillTreeMenu;
        private AbilitySkillTree abilitySkillTree;

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

            GameState = GameStateManager.LoadGameState("save.json");

            mainHUD = new MainHUD();
            gameOver = new GameOver();
            mainMenu = new MainMenu();
            skillTreeMenu = new SkillTreeMenu();
            abilitySkillTree = new AbilitySkillTree();

        #if DEBUG
            Show_Performance(false);
            showDebugData(false);
            camera.Set_min_Max_Zoom(0.03f, 1.4f);
        #endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) {
            if (GameState.AccountXP >= GameState.XPForNextLevel())
            {
                GameState.IncreaseLevel();
            }
         }

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
                case Play_State.skill_tree:
                    skillTreeMenu.Render();
                    break;
                case Play_State.ability_skill_tree:
                    abilitySkillTree.Render();
                    break;
            }
        }
    }
}