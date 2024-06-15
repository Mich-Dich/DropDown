
namespace Projektarbeit.characters.player.abilities {

    using System.Timers;
    using Core.defaults;
    using Core.world;

    public class OmniFireAbility : Ability {

        private Character character;

        public OmniFireAbility() {

            Cooldown = 15.0f; // 15 seconds cooldown
            timer = new Timer(5000); // 5 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            IconPath = "assets/textures/abilities/fireboost.png";

            Name = "OmniFire";
            Description = "Boosts your fire damage for 5 seconds.";
            UnlockCost = 30;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 30;
        }

        public override void Use(Character character) {

            this.character = character;
            Console.WriteLine("OmniFire ability used!");
            IsActive = true;

            timer.Start();
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e) {

            if (character != null)
                Console.WriteLine("OmniFire ability expired!");

            IsActive = false;
        }

    }
}
