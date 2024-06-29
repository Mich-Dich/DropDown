namespace Projektarbeit.characters.player.abilities
{
    using System.Timers;
    using Core.defaults;
    using Core.util;
    using Core.world;

    public class OmniFireAbility : Ability
    {
        private Character character;
        private readonly Timer timer;

        public OmniFireAbility()
        {
            Cooldown = 15.0f;
            Duration = 5.0f;
            Level = 1;
            timer = new Timer { AutoReset = false };
            timer.Elapsed += OnTimerElapsed;

            IconPath = "assets/textures/abilities/fireboost.png";
            Name = "OmniFire";
            Description = GetFormattedDescription();
            UnlockCost = 30;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 30;
        }

        public override void Use(Character character)
        {
            this.character = character;
            LogAbilityUse();
            IsActive = true;

            timer.Interval = Duration * 1000;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            LogAbilityExpiration();
            IsActive = false;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            Duration += 0.5f;
            timer.Interval = Duration * 1000;

            LogUpgrade();
            GameStateManager.SaveGameState(Core.Game.Instance.GameState, "save.json");
        }

        private void LogAbilityUse()
        {
            Console.WriteLine($"{Name} ability used!");
        }

        private void LogAbilityExpiration()
        {
            if (character != null)
            {
                Console.WriteLine($"{Name} ability expired!");
            }
        }

        private void LogUpgrade()
        {
            Console.WriteLine($"{Name} ability upgraded to level {Level}");
            Console.WriteLine($"{Name} duration: {Duration} seconds");
        }

        private string GetFormattedDescription()
        {
            return $"Boosts your fire damage for {Duration} seconds.";
        }
    }
}
