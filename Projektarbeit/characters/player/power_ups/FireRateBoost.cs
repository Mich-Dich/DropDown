namespace Hell.player.power
{
    using Core.defaults;
    using Core.render;
    using Core.util;
    using Core.world;
    using Hell.player;
    using OpenTK.Mathematics;

    public class FireRateBoost : PowerUp
    {
        public float FireDelayDecrease { get; set; } = 0.3f;

        private static readonly Texture Texture = new ("assets/textures/power-ups/firerate_increaser.png");
        private static readonly Vector2 Size = new (30, 30);
        private readonly float originalFireDelay = 1.0f;

        public FireRateBoost(Vector2 position)
            : base(position, Size, new Sprite(Texture))
        {
            Console.WriteLine("FireRateBoost created");
            this.IconPath = "assets/textures/abilities/fireboost.png";
            this.originalFireDelay = (Game.Instance.playerController as Hell.player.PC_main).character.fireDelay;

            this.activation = (Character target) =>
            {
                Console.WriteLine("FireRateBoost activation");
                Game.Instance.player.ActivePowerUps.Add(this);

                if (target is CH_player player)
                {
                    if (Game.Instance.playerController is Hell.player.PC_main pcMain)
                    {
                        pcMain.character.fireDelay -= this.FireDelayDecrease;
                    }
                }
            };

            this.deactivation = (Character target) =>
            {
                Console.WriteLine("FireRateBoost deactivation");
                Game.Instance.player.ActivePowerUps.Remove(this);

                if (target is CH_player player)
                {
                    if (Game.Instance.playerController is Hell.player.PC_main pcMain)
                    {
                        pcMain.character.fireDelay = this.originalFireDelay;
                    }
                }
            };
        }
    }
}
