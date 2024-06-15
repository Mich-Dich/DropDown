namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class HealthIncrease : PowerUp
    {
        public float HealthIncreaseAmount { get; set; } = 30f;

        public HealthIncrease(Vector2 position)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/health.png")))
        {
            activation = ActivatePowerUp;

            deactivation = (target) => { };

            Name = "HealthIncrease";
            Description = "gives an instant health increased";
            UnlockCost = 30;
            UpgradeMultiplier = 1.7f;
            BaseUpgradeCost = 30;
        }

        private void ActivatePowerUp(Character target)
        {
            if (target is CH_player player)
            {
                player.health += HealthIncreaseAmount;
                if (player.health > player.health_max)
                {
                    player.health = player.health_max;
                }
            }
        }
    }
}