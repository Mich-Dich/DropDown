namespace Projektarbeit
{
    using Core;
    using Projektarbeit.characters.player;
    using Projektarbeit.characters.player.power_ups;
    using Projektarbeit.characters.player.abilities;
    using Projektarbeit.Levels;
    using Projektarbeit.UI;
    using Core.world;
    using Core.util;
    using Core.defaults;

    internal class Game : Core.Game
    {
        private MainHUD mainHUD;
        private GameOver gameOver;
        private MainMenu mainMenu;
        private SkillTreeMenu skillTreeMenu;
        private AbilitySkillTree abilitySkillTree;
        private PowerupSkillTree powerupSkillTree;
        private PauseMenu pauseMenu;
        private LevelUpMenu levelUpMenu;
        private PauseMenuSkillTree pauseMenuSkillTree;
        private PauseAbilitySkillTree pauseAbilitySkillTree;
        private PausePowerupSkillTree pausePowerupSkillTree;

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
            mainHUD.clearStatusEffects();
            get_active_map().allPowerUps.Clear();
            player.ActivePowerUps.Clear();
        }

        public override List<PowerUp> loadPowerups(List<PowerUpSaveData> PowerUpsSaveData)
        {
            List<PowerUp> powerUps = new List<PowerUp>();
            foreach (var powerUpSaveData in PowerUpsSaveData)
            {
                PowerUp newPowerUp = null;
                switch(powerUpSaveData.PowerUpType)
                {
                    case "FireRateBoost":
                        newPowerUp = new FireRateBoost(new OpenTK.Mathematics.Vector2(999, 999), fireDelayDecrease: 0.1f, duration: 4f);
                        break;
                    case "HealthIncrease":
                        newPowerUp = new HealthIncrease(new OpenTK.Mathematics.Vector2(999, 999));
                        break;
                    case "SpeedBoost":
                        newPowerUp = new SpeedBoost(new OpenTK.Mathematics.Vector2(999, 999), speedIncrease: 300f, duration: 3.0f);
                        break;
                }  

                if (newPowerUp != null)
                {
                    newPowerUp.LoadFromSaveData(powerUpSaveData);

                    // Check if a power-up of the same type and level already exists in the list
                    if (!powerUps.Any(p => p.GetType() == newPowerUp.GetType() && p.Level == newPowerUp.Level))
                    {
                        powerUps.Add(newPowerUp);
                    }
                }
            }

            return powerUps;
        }

        public override List<Ability> loadAbilities(List<AbilitySaveData> AbilitesSaveData)
        {
            List<Ability> abilities = new List<Ability>();
            foreach (var abilitySaveData in AbilitesSaveData)
            {
                switch(abilitySaveData.AbilityType)
                {
                    case "OmniFireAbility":
                        var omniFireAbility = new OmniFireAbility();
                        omniFireAbility.LoadFromSaveData(abilitySaveData);
                        abilities.Add(omniFireAbility);
                        break;
                    case "ShieldAbility":
                        var shieldAbility = new ShieldAbility();
                        shieldAbility.LoadFromSaveData(abilitySaveData);
                        abilities.Add(shieldAbility);
                        break;
                    case "TestAbility":
                        break;
                }  
            }

            return abilities;
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
            powerupSkillTree = new PowerupSkillTree();
            pauseMenu = new PauseMenu();
            levelUpMenu = new LevelUpMenu();
            pauseMenuSkillTree = new PauseMenuSkillTree();
            pauseAbilitySkillTree = new PauseAbilitySkillTree();
            pausePowerupSkillTree = new PausePowerupSkillTree();

        #if DEBUG
            Show_Performance(false);
            showDebugData(false);
            camera.Set_min_Max_Zoom(0.03f, 1.4f);
        #endif
        }

        protected override void Shutdown() { }

        protected override void Update(float deltaTime) 
        {
            if (GameState.AccountXP >= GameState.XPForNextLevel())
            {
                GameState.IncreaseLevel();
            }

            GameState.RemoveDuplicatePowerUps();
        }

        protected override void Render(float deltaTime) { }

        protected override void Render_Imgui(float deltaTime)
        {
            var oldState = play_state;

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
                case Play_State.powerup_skill_tree:
                    powerupSkillTree.Render();
                    break;
                case Play_State.InGameMenu:
                    pauseMenu.Render();
                    break;
                case Play_State.LevelUp:
                    levelUpMenu.Render();
                    break;
                case Play_State.PauseMenuSkillTree:
                    pauseMenuSkillTree.Render();
                    break;
                case Play_State.PauseAbilitySkillTree:
                    pauseAbilitySkillTree.Render();
                    break;
                case Play_State.PausePowerupSkillTree:
                    pausePowerupSkillTree.Render();
                    break;
            }

            if (oldState != play_state)
            {
                OnGameStateChanged(oldState, play_state);
            }
        }
    }
}