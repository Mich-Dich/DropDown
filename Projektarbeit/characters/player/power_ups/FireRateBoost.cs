namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;
    using Core.util;

    public class FireRateBoost : PowerUp {

        public float FireDelayDecrease { get; set; } = 0.1f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/firerate_increaser.png");
        private static readonly Vector2 size = new Vector2(30, 30);
        private float originalFireDelay;

        public FireRateBoost(Vector2 position) : base(position, size, new Sprite(texture))  {
        
            Console.WriteLine("FireRateBoost created");

            activation = (Character target) => {

                if(target is CH_player player)
                    if(Game.Instance.playerController is Hell.player.PC_main pcMain)
                        pcMain.fireDelay -= FireDelayDecrease;
            };

            deactivation = (Character target) => {

                if(target is CH_player player)
                    if(Game.Instance.playerController is Hell.player.PC_main pcMain)
                        pcMain.fireDelay = originalFireDelay;
            };

        }

    }
}
