namespace Hell.player.power {
    using Core.defaults;
    using OpenTK.Mathematics;
    using Core.world;
    using Hell.player;
    using Core.render;

    public class HealthBoost : PowerUp {

        public float HealthIncrease { get; set; } = 100f;
        private static readonly Texture texture = new Texture("assets/textures/power-ups/health.png");
        private static readonly Vector2 size = new Vector2(30, 30);

        public HealthBoost(Vector2 position) : base(position, size, new Sprite(texture)) 
        {
            Console.WriteLine("HealthBoost created");
            ActivationTime = DateTime.Now;
         }

        public override void Activate(Game_Object target) {
            if(target is CH_player player) {
                player.health_max += HealthIncrease;
                Game.Instance.get_active_map().AddPowerUp(this);
                Console.WriteLine("HealthBoost activated");
            }
        }

        public override void Deactivate(Game_Object target) {
            if(target is CH_player player) {
                player.health_max -= HealthIncrease;
                Game.Instance.get_active_map().RemovePowerUp(this);
                Console.WriteLine("HealthBoost deactivated");
            }
        }
    }
}