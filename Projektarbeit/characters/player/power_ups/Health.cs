namespace Hell.player.power
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using Hell.player;
    using OpenTK.Mathematics;

    public class HealthIncrease : PowerUp
    {
        public float HealthIncreaseAmount { get; set; } = 30f;

        private static readonly Texture Texture = new ("assets/textures/power-ups/health.png");
        private static readonly Vector2 Size = new (30, 30);

        public HealthIncrease(Vector2 position)
            : base(position, Size, new Sprite(Texture))
        {
            Console.WriteLine("HealthIncrease created");

            this.activation = (Character target) =>
            {
                if (target is CH_player player)
                {
                    player.health += this.HealthIncreaseAmount;
                    if (player.health > player.health_max)
                    {
                        player.health = player.health_max;
                    }
                }
            };

            this.deactivation = (Character target) =>
            {
                // No deactivation effect for this power-up
            };
        }
    }
}