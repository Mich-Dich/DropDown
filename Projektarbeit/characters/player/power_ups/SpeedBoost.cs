namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;

    public class SpeedBoost : PowerUp {

        public float SpeedIncrease { get; set; } = 500f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/speed_increaser.png");
        private static readonly Vector2 size = new Vector2(30, 30);
        private float testVar = 0;

        public SpeedBoost(Vector2 position) : base(position, size, new Sprite(texture)) 
        {
            Console.WriteLine("SpeedBoost created");
            ActivationTime = DateTime.Now;
         }

        public override void Activate(Game_Object target) {
            if(target is CH_player player) {
                testVar = player.movement_speed;
                player.movement_speed += SpeedIncrease;
                Game.Instance.get_active_map().AddPowerUp(this);
                Console.WriteLine("SpeedBoost activated");
            }
        }

        public override void Deactivate(Game_Object target) {
            if(target is CH_player player) {
                player.movement_speed = testVar;
                Game.Instance.get_active_map().RemovePowerUp(this);
                Console.WriteLine("SpeedBoost deactivated");
            }
        }
    }
}