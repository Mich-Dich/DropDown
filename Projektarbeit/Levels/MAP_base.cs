
namespace Projektarbeit.Levels {

    using Core.util;
    using Core.world;
    using Core.world.map;
    using OpenTK.Mathematics;
    using Hell.enemy;

    internal class MAP_base : Map {

        private Camera camera;
        private Random random;
        private float time_stamp;
        private float time_interval;

        public MAP_base() {

            this.camera = Game.Instance.camera;
            this.random = new Random();

            time_stamp = Game_Time.total;
            time_interval = random.Next(2,4);

            Set_Background_Image("assets/textures/background/background.png");
        }

        public override void update(float deltaTime) {

            if((time_stamp + time_interval) <= Game_Time.total) {

                Add_Character(new TestEnemyController(new TestEnemy()), new Vector2(100, 100), 0);
                time_stamp = Game_Time.total;
                time_interval = random.Next(2, 4);
            }
        }

    }
}
