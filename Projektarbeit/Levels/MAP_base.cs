
namespace Hell.Levels {

    using Core.util;
    using Core.world;
    using Core.world.map;
    using OpenTK.Mathematics;
    using Hell.enemy;
    using Hell.player.power;

    internal class MAP_base : Map {

        private Camera camera;
        private Random random;
        private float time_stamp;
        private float time_interval;

        bool is_running = false;

        public MAP_base() {

            this.camera = Game.Instance.camera;
            this.random = new Random();

            time_stamp = Game_Time.total;
            time_interval = random.Next(2,4);

            Set_Background_Image("assets/textures/background/background.png", 1.18f);

        }

        public override void update(float deltaTime) {

            if((time_stamp + time_interval) <= Game_Time.total) {

                if(!is_running) {

                    add_AI_Controller(new SwarmEnemyController(new Vector2(0, -500)));

                    SpeedBoost speedBoost = new SpeedBoost(new Vector2(0, 300));
                    Add_Game_Object(speedBoost);

                    //FireRateBoost fireRateBoost = new FireRateBoost(new Vector2(0, 400));
                    //Add_Game_Object(fireRateBoost);

                    time_stamp = Game_Time.total;
                    time_interval = random.Next(2, 4);
                    is_running = true;
                }
            }
        }

    }
}
