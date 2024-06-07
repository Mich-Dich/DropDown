namespace Projektarbeit.characters.player.power_ups
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.player;

    public class HealthBoost : PowerUp
    {
        public float HealthIncrease { get; set; } = 100f;

        public HealthBoost(Vector2 position)
            : base(position, new Vector2(30, 30), new Sprite(new Texture("assets/textures/power-ups/health.png")))
        {
            activation = ActivatePowerUp;

            deactivation = DeactivatePowerUp;
        }

        private void ActivatePowerUp(Character target)
        {
            if (target is CH_player player)
            {
                player.health_max += HealthIncrease;
            }
        }

        private void DeactivatePowerUp(Character target)
        {
            if (target is CH_player player)
            {
                player.health_max -= HealthIncrease;
            }
        }
    }
}
