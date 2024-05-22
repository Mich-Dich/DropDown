namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;

    public class FireRateBoost : PowerUp {

        public float FireDelayDecrease { get; set; } = 0.1f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/firerate_increaser.png");
        private static readonly Vector2 size = new Vector2(30, 30);
        private float originalFireDelay;

        public FireRateBoost(Vector2 position) : base(position, size, new Sprite(texture)) 
        {
            Console.WriteLine("FireRateBoost created");
            ActivationTime = DateTime.Now;
         }

        public override void Activate(Game_Object target) {
            if(target is CH_player player) {
                if (Game.Instance.playerController is Hell.player.PC_main pcMain) {
                    originalFireDelay = pcMain.fireDelay;
                    pcMain.fireDelay -= FireDelayDecrease;
                }
                Game.Instance.get_active_map().AddPowerUp(this);
                Console.WriteLine("FireRateBoost activated");
            }
        }

        public override void Deactivate(Game_Object target) {
            if(target is CH_player player) {
                if (Game.Instance.playerController is Hell.player.PC_main pcMain) {
                    pcMain.fireDelay = originalFireDelay;
                }
                Game.Instance.get_active_map().RemovePowerUp(this);
                Console.WriteLine("FireRateBoost deactivated");
            }
        }
    }
}