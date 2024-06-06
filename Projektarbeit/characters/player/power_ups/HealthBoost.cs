namespace Hell.player.power
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using Hell.player;
    using OpenTK.Mathematics;

    public class HealthBoost : PowerUp
    {
        public float HealthIncrease { get; set; } = 100f;

        private static readonly Texture Texture = new ("assets/textures/power-ups/health.png");
        private static readonly Vector2 Size = new (30, 30);

        public HealthBoost(Vector2 position)
            : base(position, Size, new Sprite(Texture))
        {
            Console.WriteLine("HealthBoost created");

            this.activation = (Character target) =>
            {
                if (target is CH_player player)
                {
                    player.health_max += this.HealthIncrease;
                }
            };

            this.deactivation = (Character target) =>
            {
                if (target is CH_player player)
                {
                    player.health_max -= this.HealthIncrease;
                }
            };
        }
    }
}