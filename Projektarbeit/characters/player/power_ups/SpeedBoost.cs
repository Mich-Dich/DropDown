namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class SpeedBoost : PowerUp
    {
        private float customSpeedIncrease;
        private float customDuration;

        public SpeedBoost(Vector2 position, float speedIncrease, float duration)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/speed_increaser.png")))
        {
            IconPath = "assets/textures/power-ups/speed_increaser.png";

            activation = ActivatePowerUp;
            deactivation = DeactivatePowerUp;
            destruction = () => { };

            Name = "SpeedBoost";
            Description = "Increases the player's speed for a period of time.";
            UnlockCost = 35;
            UpgradeMultiplier = 1.5f;
            BaseUpgradeCost = 35;

            this.customSpeedIncrease = speedIncrease;
            this.customDuration = duration;
            Duration = duration; // Set the base class duration
        }

        public override void Upgrade()
        {
            base.Upgrade();

            customSpeedIncrease += 100;
            customDuration = Level % 2 != 0 ? customDuration + 1 : customDuration;
            Duration = customDuration; // Update the base class duration

            Console.WriteLine($"SpeedBoost upgraded to level {Level}");
            Console.WriteLine($"SpeedBoost: {customSpeedIncrease} activated for {customDuration} seconds");

            GameStateManager.SaveGameState(Game.Instance.GameState, "save.json");
        }

        private void ActivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Add(this);
            Console.WriteLine("SpeedBoost activated");

            if (target is CH_player player)
            {
                float oldSpeed = player.movement_speed;
                player.movement_speed += customSpeedIncrease;
                Console.WriteLine($"SpeedBoost: {oldSpeed} -> {player.movement_speed} (+{customSpeedIncrease}) for {customDuration} seconds Level {Level}");
            }
        }

        private void DeactivatePowerUp(Character target)
        {
            Core.Game.Instance.player.ActivePowerUps.Remove(this);

            if (target is CH_player player)
            {
                float oldSpeed = player.movement_speed;
                player.movement_speed -= customSpeedIncrease;
                Console.WriteLine($"SpeedBoost deactivated: {oldSpeed} -> {player.movement_speed} (-{customSpeedIncrease})");
            }
        }
    }
}
