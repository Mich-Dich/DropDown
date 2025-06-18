namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class HealthIncrease : PowerUp
    {
        public HealthIncrease(Vector2 position)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/health.png")))
        {
            activation = ActivatePowerUp;
            deactivation = (target) => { };
            destruction = () => { };
            Name = "HealthIncrease";
            Description = "Gives an instant health increase";
            UnlockCost = 30;
            UpgradeMultiplier = 1.7f;
            BaseUpgradeCost = 30;
            HealthIncreaseAmount = 30f;
            Duration = 0f; // Instant effect, no duration
            IconPath = "assets/textures/power-ups/health.png";
        }

        public override void Upgrade()
        {
            base.Upgrade();
            HealthIncreaseAmount += 5;
            Console.WriteLine($"HealthIncrease upgraded to level {Level}");
            Console.WriteLine($"HealthIncrease: +{HealthIncreaseAmount} health");
        }

        private void ActivatePowerUp(Character target)
        {
            if (target is CH_player player)
            {
                float oldHealth = player.health;
                player.health += HealthIncreaseAmount;
                if (player.health > player.health_max)
                {
                    player.health = player.health_max;
                }
                
                Console.WriteLine($"HealthIncrease activated: {oldHealth} -> {player.health} (+{HealthIncreaseAmount})");
            }
        }
    }
}