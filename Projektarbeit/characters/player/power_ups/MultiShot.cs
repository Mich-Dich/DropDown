namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;
    using Core.util;

    public class MultiShot : PowerUp {

        public int ExtraProjectiles { get; set; } = 2;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/multishot.png");
        private static readonly Vector2 size = new Vector2(30, 30);
        private int originalProjectiles = 1;

        public MultiShot(Vector2 position) : base(position, size, new Sprite(texture))  {
        
            Console.WriteLine("MultiShot created");

            activation = (Character target) => {
                if(target is CH_player player)
                    player.projectilesPerShot += ExtraProjectiles;
            };

            deactivation = (Character target) => {
                if(target is CH_player player)
                    player.projectilesPerShot = originalProjectiles;
            };

        }

    }
}