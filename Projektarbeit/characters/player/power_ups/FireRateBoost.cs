namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class FireRateBoost : PowerUp
    {
        private float originalFireDelay;
        private float CustomFireDelayDecrease;
        private float CustomDuration;


        public FireRateBoost(Vector2 position, float fireDelayDecrease, float duration)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/firerate_increaser.png")))
        {
            IconPath = "assets/textures/abilities/fireboost.png";

            activation = ActivatePowerUp;

            deactivation = DeactivatePowerUp;

            Name = "FireRateBoost";
            Description = "Decreases the fire delay for 2 seconds.";
            UnlockCost = 20;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 20;


            this.FireDelayDecrease = 0.1f;
            this.Duration = 2f;

            this.CustomFireDelayDecrease = fireDelayDecrease;
            this.CustomDuration = duration;

            this.Duration = CustomDuration;
            
        }

        public override void Upgrade()
        {
            base.Upgrade();

            CustomFireDelayDecrease += 0.1f;
            CustomDuration = Level % 2 != 0 ? CustomDuration + 1 : CustomDuration;

            // Update the properties in the PowerUp class
            this.FireDelayDecrease = CustomFireDelayDecrease;
            this.Duration = CustomDuration;

            Console.WriteLine("FireRateBoost upgraded to level " + Level);
            Console.WriteLine("FireRateBoost: " + FireDelayDecrease + " activated for " + CustomDuration + " seconds");

            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        private void ActivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Add(this);
            Console.WriteLine("SpeedBoost activated");

            if (target is CH_player player)
            {
                if (Core.Game.Instance.playerController is PC_main pcMain)
                {
                    originalFireDelay = pcMain.character.fireDelay;
                    pcMain.character.fireDelay -= CustomFireDelayDecrease;
                }
            }

            Console.WriteLine("SpeedBoost: " + CustomFireDelayDecrease + " activated for " + CustomDuration + " seconds");
        }

        private void DeactivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Remove(this);

            if (target is CH_player player)
            {
                if (Core.Game.Instance.playerController is PC_main pcMain)
                {
                    pcMain.character.fireDelay = originalFireDelay;
                }
            }
        }
    }
}
