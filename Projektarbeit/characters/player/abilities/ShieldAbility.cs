namespace Projektarbeit.characters.player.abilities
{
    using System.Timers;
    using Core.defaults;
    using Core.world;

    public class ShieldAbility : Ability
    {
        private readonly Timer timer;
        private Character character;

        public ShieldAbility()
        {
            Cooldown = 10.0f; // 10 seconds cooldown
            timer = new Timer(2500); // 2 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;

            float scale = 1.6f;
            int fps = 8;
            bool loop = true;

            Effect = new AbilityEffect("assets/animation/shield/shield.png", scale, 4, 1, fps, loop);
            IconPath = "assets/textures/abilities/shield.png";

            Name = "Shield";
            Description = "Makes you invincible for 2 seconds.";
            UnlockCost = 20;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 20;
        }

        public override void Use(Character character)
        {
            this.character = character;
            character.Invincible = true;
            timer.Start();
            Console.WriteLine("Shield ability used!");

            AddEffectToCharacter(character);

            Core.Game.Instance.get_active_map().Add_Game_Object(Effect);
            IsActive = true;
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
    }
}