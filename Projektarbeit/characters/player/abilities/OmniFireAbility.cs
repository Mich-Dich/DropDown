namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;
    using System.Timers;

    public class OmniFireAbility : Ability
    {
        private Timer timer;
        private Character character;

        public OmniFireAbility() {
            Cooldown = 15.0f; // 15 seconds cooldown
            timer = new Timer(5000); // 5 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            this.IconPath = "assets/textures/abilities/fireboost.png";
        }

        public override void Use(Character character) {
            this.character = character;
            timer.Start();
            Console.WriteLine("OmniFire ability used!");
            IsActive = true;
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e) {
            if (character != null) {
                Console.WriteLine("OmniFire ability expired!");
            }
            IsActive = false;
        }
    }
}