namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class FireRateBoost : PowerUp
    {
        public float FireDelayDecrease { get; set; } = 0.3f;

        private float originalFireDelay;

        public FireRateBoost(Vector2 position)
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
        }

        private void ActivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Add(this);

            if (target is CH_player player)
            {
                if (Core.Game.Instance.playerController is PC_main pcMain)
                {
                    originalFireDelay = pcMain.character.fireDelay;
                    pcMain.character.fireDelay -= FireDelayDecrease;
                }
            }
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
