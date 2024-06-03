
namespace Hell.player.power {

    using Core.defaults;
    using Core.render;
    using Core.world;
    using Hell.player;
    using OpenTK.Mathematics;

    public class SpeedBoost : PowerUp {

        public float SpeedIncrease { get; set; } = 1000f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/speed_increaser.png");
        private static readonly Vector2 size = new Vector2(30, 30);

        public SpeedBoost(Vector2 position) : base(position, size, new Sprite(texture)) {

            Console.WriteLine("SpeedBoost created");
            IconPath = "assets/textures/abilities/fireboost.png";
            activation = (Character target) => {
                Game.Instance.player.ActivePowerUp = this;

                if(target is CH_player player)
                    player.movement_speed += SpeedIncrease;

            };

            deactivation = (Character target) => {
                Console.WriteLine("SpeedBoost deactivation");
                Game.Instance.player.ActivePowerUp = null;
                
                if(target is CH_player player)
                    player.movement_speed -= SpeedIncrease;
            };
        }


    }
}
