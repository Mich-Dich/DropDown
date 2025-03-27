namespace Hell.player.power
{
    using Core.defaults;
    using Core.render;
    using Core.world;
    using Hell.player;
    using OpenTK.Mathematics;

    public class SpeedBoost : PowerUp
    {
        public float SpeedIncrease { get; set; } = 1000f;

        private static readonly Texture Texture = new ("assets/textures/power-ups/speed_increaser.png");
        private static readonly Vector2 Size = new (30, 30);

        public SpeedBoost(Vector2 position)
            : base(position, Size, new Sprite(Texture))
        {
            Console.WriteLine("SpeedBoost created");
            this.IconPath = "assets/textures/abilities/fireboost.png";
            this.activation = (Character target) =>
            {
                Game.Instance.player.ActivePowerUps.Add(this);

                if (target is CH_player player)
                {
                    player.movement_speed += this.SpeedIncrease;
                }
            };

            this.deactivation = (Character target) =>
            {
                Console.WriteLine("SpeedBoost deactivation");
                Game.Instance.player.ActivePowerUps.Remove(this);

                if (target is CH_player player)
                {
                    player.movement_speed -= this.SpeedIncrease;
                }
            };
        }
    }
}
