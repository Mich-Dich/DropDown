namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;
    using System.Timers;

    public class OmniFireAbility : Ability
    {
        private Timer timer;
        private Character character;
        private bool isActive;

        public OmniFireAbility() {
            Cooldown = 15.0f; // 15 seconds cooldown
            timer = new Timer(5000); // 5 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            isActive = false;
        }

        public override void Use(Character character) {
            this.character = character;
            isActive = true;
            timer.Start();
            Console.WriteLine("OmniFire ability used!");
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e) {
            if (character != null) {
                isActive = false;
                Console.WriteLine("OmniFire ability expired!");
            }
        }

        public bool IsActive() {
            return isActive;
        }
    }
}