namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class SpeedBoost : PowerUp
    {
        public float SpeedIncrease { get; set; } = 400f;

        public SpeedBoost(Vector2 position)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/speed_increaser.png")))
        {
            IconPath = "assets/textures/abilities/fireboost.png";

            activation = ActivatePowerUp;

            deactivation = DeactivatePowerUp;

            Name = "SpeedBoost";
            Description = "increases the players speed for a period of time";
            UnlockCost = 35;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 35;

            SpeedBoost = 400f;
            Duration = 3f;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            SpeedIncrease += 100;
            DurationBoost = 1; // Set DurationBoost to 1
            Duration = Level % 2 != 0 ? Duration + DurationBoost : Duration;

            // Update the properties in the PowerUp class
            this.SpeedBoost = SpeedIncrease;
            this.Duration = Duration;

            Console.WriteLine("SpeedBoost upgraded to level " + Level);
            Console.WriteLine("SpeedBoost: " + SpeedIncrease + " activated for " + Duration + " seconds");

            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        private void ActivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Add(this);
            Console.WriteLine("SpeedBoost activated");

            if (target is CH_player player)
            {
                player.movement_speed += SpeedIncrease;
            }

            Console.WriteLine("SpeedBoost: " + SpeedIncrease + " activated for " + Duration + " seconds");
        }

        private void DeactivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Remove(this);

            if (target is CH_player player)
            {
                player.movement_speed -= SpeedIncrease;
            }
        }
    }
}