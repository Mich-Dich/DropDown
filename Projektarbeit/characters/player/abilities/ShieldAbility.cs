namespace Hell.player.ability
{
    using System.Timers;
    using Core.defaults;
    using Core.render;
    using Core.world;

    public class ShieldAbility : Ability
    {
        private readonly Timer timer;
        private Character character;

        public ShieldAbility()
        {
            this.Cooldown = 10.0f; // 10 seconds cooldown
            this.timer = new Timer(2500); // 2 seconds duration
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.AutoReset = false;

            float scale = 1.6f;
            int fps = 8;
            bool loop = true;

            this.Effect = new AbilityEffect("assets/animation/shield/shield.png", scale, 4, 1, fps, loop);
            this.IconPath = "assets/textures/abilities/shield.png";
        }

        public override void Use(Character character)
        {
            this.character = character;
            character.Invincible = true;
            this.timer.Start();
            Console.WriteLine("Shield ability used!");

            this.AddEffectToCharacter(character);

            Game.Instance.get_active_map().Add_Game_Object(this.Effect);
            this.IsActive = true;
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e)
        {
            if (this.character != null)
            {
                this.character.Invincible = false;
                Console.WriteLine("Shield ability expired!");

                Game.Instance.get_active_map().Remove_Game_Object(this.Effect);
                this.IsActive = false;
            }
        }
    }
}