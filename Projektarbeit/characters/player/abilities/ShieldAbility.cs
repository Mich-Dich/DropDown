namespace Hell.player.ability {
    using Core.defaults;
    using Core.world;
    using Core.render;
    using System.Timers;

    public class ShieldAbility : Ability
    {
        private Timer timer;
        private Character character;

        public ShieldAbility() {
            Cooldown = 10.0f; // 10 seconds cooldown
            timer = new Timer(2500); // 2 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;

            float scale = 1.6f;
            int fps = 8;
            bool loop = true;

            this.Effect = new AbilityEffect("assets/animation/shield/shield.png", scale, 4, 1, fps, loop);
            this.IconPath = "assets/textures/abilities/shield.png";
        }

        public override void Use(Character character) {
            this.character = character;
            character.Invincible = true;
            timer.Start();
            Console.WriteLine("Shield ability used!");

            AddEffectToCharacter(character);

            Game.Instance.get_active_map().Add_Game_Object(this.Effect);
            IsActive = true;
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e) {
            if (character != null) {
                character.Invincible = false;
                Console.WriteLine("Shield ability expired!");

                Game.Instance.get_active_map().Remove_Game_Object(this.Effect);
                IsActive = false;
            }
        }
    }
}