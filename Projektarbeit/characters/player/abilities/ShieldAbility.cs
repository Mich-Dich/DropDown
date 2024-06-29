namespace Projektarbeit.characters.player.abilities
{
    using System.Timers;
    using Core.defaults;
    using Core.util;
    using Core.world;

    public class ShieldAbility : Ability
    {
        private Character character;
        private readonly Timer timer;

        public ShieldAbility()
        {
            InitializeAbility();
            timer = new Timer { AutoReset = false };
            timer.Elapsed += OnTimerElapsed;
        }

        public override void Use(Character character)
        {
            this.character = character;
            character.Invincible = true;
            LogAbilityUse();

            AddEffectToCharacter(character);
            Core.Game.Instance.get_active_map().Add_Game_Object(Effect);
            IsActive = true;

            timer.Interval = Duration * 1000;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (character != null)
            {
                character.Invincible = false;
                LogAbilityExpiration();
                Core.Game.Instance.get_active_map().Remove_Game_Object(Effect);
                IsActive = false;
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            Duration += 0.5f;
            timer.Interval = Duration * 1000;

            LogUpgrade();
            GameStateManager.SaveGameState(Core.Game.Instance.GameState, "save.json");
        }

        private void InitializeAbility()
        {
            Cooldown = 10.0f;
            Duration = 2.0f;
            Level = 1;

            Effect = new AbilityEffect("assets/animation/shield/shield.png", 1.6f, 4, 1, 8, true);
            IconPath = "assets/textures/abilities/shield.png";

            Name = "Shield";
            Description = GetFormattedDescription();
            UnlockCost = 20;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 20;
        }

        private void LogAbilityUse()
        {
            Console.WriteLine($"{Name} ability used!");
            Console.WriteLine($"Player is invincible for {Duration} seconds.");
        }

        private void LogAbilityExpiration()
        {
            Console.WriteLine($"{Name} ability expired!");
        }

        private void LogUpgrade()
        {
            Console.WriteLine($"{Name} ability upgraded to level {Level}");
            Console.WriteLine($"{Name} duration: {Duration} seconds");
        }

        private string GetFormattedDescription()
        {
            return $"Makes you invincible for {Duration} seconds.";
        }
    }
}
