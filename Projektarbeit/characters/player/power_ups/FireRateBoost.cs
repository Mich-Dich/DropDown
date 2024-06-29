namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class FireRateBoost : PowerUp
    {
        private float originalFireDelay;

        public FireRateBoost(Vector2 position, float fireDelayDecrease, float duration)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/firerate_increaser.png")))
        {
            IconPath = "assets/textures/abilities/fireboost.png";
            activation = ActivatePowerUp;
            deactivation = DeactivatePowerUp;

            Name = "FireRateBoost";
            Description = "Decreases the fire delay.";
            UnlockCost = 20;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 20;

            FireDelayDecrease = fireDelayDecrease;
            Duration = duration;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            FireDelayDecrease += 0.1f;
            Duration = Level % 2 != 0 ? Duration + 1 : Duration;

            LogUpgradeDetails();
            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        private void ActivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Add(this);
            LogActivationDetails();

            if (Core.Game.Instance.playerController is PC_main pcMain)
            {
                originalFireDelay = pcMain.character.fireDelay;
                pcMain.character.fireDelay -= FireDelayDecrease;
            }
        }

        private void DeactivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Remove(this);

            if (Core.Game.Instance.playerController is PC_main pcMain)
            {
                pcMain.character.fireDelay = originalFireDelay;
            }
        }

        private void LogUpgradeDetails()
        {
            Console.WriteLine($"FireRateBoost upgraded to level {Level}");
            Console.WriteLine($"FireRateBoost: {FireDelayDecrease} activated for {Duration} seconds");
        }

        private void LogActivationDetails()
        {
            Console.WriteLine("FireRateBoost activated");
            Console.WriteLine($"FireRateBoost: {FireDelayDecrease} activated for {Duration} seconds");
        }
    }
}
