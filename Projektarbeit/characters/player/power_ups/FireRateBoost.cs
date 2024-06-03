namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;
    using Core.util;

    public class FireRateBoost : PowerUp {

        public float FireDelayDecrease { get; set; } = 0.3f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/firerate_increaser.png");
        private static readonly Vector2 size = new Vector2(30, 30);
        private float originalFireDelay = 1.0f;

        public FireRateBoost(Vector2 position) : base(position, size, new Sprite(texture))  {
        
            Console.WriteLine("FireRateBoost created");
            IconPath = "assets/textures/abilities/fireboost.png";
            originalFireDelay = (Game.Instance.playerController as Hell.player.PC_main).character.fireDelay;

            activation = (Character target) => {
                Console.WriteLine("FireRateBoost activation");
                Game.Instance.player.ActivePowerUps.Add(this);

                if(target is CH_player player)
                    if(Game.Instance.playerController is Hell.player.PC_main pcMain)
                        pcMain.character.fireDelay -= FireDelayDecrease;
            };

            deactivation = (Character target) => {
                Console.WriteLine("FireRateBoost deactivation");
                Game.Instance.player.ActivePowerUps.Remove(this);

                if(target is CH_player player)
                    if(Game.Instance.playerController is Hell.player.PC_main pcMain)
                        pcMain.character.fireDelay = originalFireDelay;
            };

        }

    }
}
