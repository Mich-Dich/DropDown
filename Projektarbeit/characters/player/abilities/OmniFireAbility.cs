namespace Hell.player.ability
{
    using System.Timers;
    using Core.defaults;
    using Core.world;

    public class OmniFireAbility : Ability
    {
        private readonly Timer timer;
        private Character character;

        public OmniFireAbility()
        {
            this.Cooldown = 15.0f; // 15 seconds cooldown
            this.timer = new Timer(5000); // 5 seconds duration
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.AutoReset = false;
            this.IconPath = "assets/textures/abilities/fireboost.png";
        }

        public override void Use(Character character)
        {
            this.character = character;
            this.timer.Start();
            Console.WriteLine("OmniFire ability used!");
            this.IsActive = true;
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e)
        {
            if (this.character != null)
            {
                Console.WriteLine("OmniFire ability expired!");
            }

            this.IsActive = false;
        }
    }
}