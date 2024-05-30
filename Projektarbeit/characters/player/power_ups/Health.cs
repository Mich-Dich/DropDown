namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;

    public class HealthIncrease : PowerUp {

        public float HealthIncreaseAmount { get; set; } = 30f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/health.png");
        private static readonly Vector2 size = new Vector2(30, 30);

        public HealthIncrease(Vector2 position) : base(position, size, new Sprite(texture))  {
            
            Console.WriteLine("HealthIncrease created");

            activation = (Character target) => {

                if(target is CH_player player) {
                    player.health += HealthIncreaseAmount;
                    if (player.health > player.health_max) {
                        player.health = player.health_max;
                    }
                }
            };

            deactivation = (Character target) => {
                // No deactivation effect for this power-up
            };

        }

    }
}