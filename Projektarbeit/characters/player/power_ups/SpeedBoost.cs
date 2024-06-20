namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class SpeedBoost : PowerUp
    {
        private float CustomSpeedIncrease;
        private float CustomDuration;

        public SpeedBoost(Vector2 position, float speedIncrease, float duration)
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

            this.SpeedBoost = 300.0f;
            this.Duration = 3.0f;

            this.CustomSpeedIncrease = speedIncrease;
            this.CustomDuration = duration;

            this.Duration = CustomDuration;
        }

        public override void Upgrade()
        {
            base.Upgrade();

            CustomSpeedIncrease += 100;
            Duration = Level % 2 != 0 ? Duration + 1 : Duration;

            // Update the properties in the PowerUp class
            this.SpeedBoost = CustomSpeedIncrease;
            this.Duration = Duration;

            Console.WriteLine("SpeedBoost upgraded to level " + Level);
            Console.WriteLine("SpeedBoost: " + SpeedBoost + " activated for " + Duration + " seconds");

            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        private void ActivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Add(this);
            Console.WriteLine("SpeedBoost activated");

            if (target is CH_player player)
            {
                player.movement_speed += this.CustomSpeedIncrease;
            }

            Console.WriteLine("SpeedBoost: " + this.CustomSpeedIncrease + " activated for " + this.CustomDuration + " seconds Level " + this.Level);
        }

        private void DeactivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Remove(this);

            if (target is CH_player player)
            {
                player.movement_speed -= CustomSpeedIncrease;
            }
        }
    }
}