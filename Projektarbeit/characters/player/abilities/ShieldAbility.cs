namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;
    using System.Timers;

    public class ShieldAbility : Ability
    {
        private Timer timer;
        private Character character;

        public ShieldAbility() {
            Cooldown = 10.0f; // 10 seconds cooldown
            timer = new Timer(2000); // 2 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
        }

        public override void Use(Character character) {
            this.character = character;
            character.Invincible = true;
            timer.Start();
            Console.WriteLine("Shield ability used!");
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e) {
            if (character != null) {
                character.Invincible = false;
                Console.WriteLine("Shield ability expired!");
            }
        }
    }
}