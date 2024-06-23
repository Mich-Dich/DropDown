namespace Projektarbeit.characters.player.abilities
{
    using System.Timers;
    using Core.defaults;
    using Core.world;

    public class ShieldAbility : Ability
    {
        private Character character;

        public ShieldAbility() : base()
        {
            Cooldown = 10.0f;
            Duration = 2.0f;
            Level = 1;
            timer = new Timer(Duration * 1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;

            float scale = 1.6f;
            int fps = 8;
            bool loop = true;

            Effect = new AbilityEffect("assets/animation/shield/shield.png", scale, 4, 1, fps, loop);
            IconPath = "assets/textures/abilities/shield.png";

            Name = "Shield";
            Description = $"Makes you invincible for {Duration} seconds.";
            UnlockCost = 20;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 20;
        }

        public override void Use(Character character)
        {
            this.character = character;
            character.Invincible = true;
            Console.WriteLine("Shield ability used!");
            Console.WriteLine("Player is invincible for "+ this.Duration +" seconds.");

            AddEffectToCharacter(character);

            Core.Game.Instance.get_active_map().Add_Game_Object(Effect);
            IsActive = true;

            timer.Interval = Duration * 1000;
            timer.Start();
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e)
        {
            if (character != null)
            {
                character.Invincible = false;
                Console.WriteLine("Shield ability expired!");

                Core.Game.Instance.get_active_map().Remove_Game_Object(Effect);
                IsActive = false;
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            Duration += 0.5f;
            timer.Interval = Duration * 1000;

            Console.WriteLine($"Shield ability upgraded to level {Level}");
            Console.WriteLine($"Shield duration: {Duration} seconds");

            GameStateManager.SaveGameState(Core.Game.Instance.GameState, "save.json");
        }
    }
}