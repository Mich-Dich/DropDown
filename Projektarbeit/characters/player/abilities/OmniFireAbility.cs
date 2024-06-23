namespace Projektarbeit.characters.player.abilities
{
    using System.Timers;
    using Core.defaults;
    using Core.world;

    public class OmniFireAbility : Ability
    {
        private Character character;

        public OmniFireAbility()
        {
            Cooldown = 15.0f;
            Duration = 5.0f;
            Level = 1;
            timer = new Timer(Duration * 1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;

            IconPath = "assets/textures/abilities/fireboost.png";

            Name = "OmniFire";
            Description = $"Boosts your fire damage for {Duration} seconds.";
            UnlockCost = 30;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 30;
        }

        public override void Use(Character character)
        {
            this.character = character;
            Console.WriteLine("OmniFire ability used!");
            IsActive = true;

            timer.Interval = Duration * 1000;
            timer.Start();
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e)
        {
            if (character != null)
            {
                Console.WriteLine("OmniFire ability expired!");
            }

            IsActive = false;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            Duration += 0.5f;
            timer.Interval = Duration * 1000;

            Console.WriteLine($"OmniFire ability upgraded to level {Level}");
            Console.WriteLine($"OmniFire duration: {Duration} seconds");

            GameStateManager.SaveGameState(Core.Game.Instance.GameState, "save.json");
        }
    }
}