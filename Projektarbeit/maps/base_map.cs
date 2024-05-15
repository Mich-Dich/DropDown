namespace Hell {
    using Core.world;
    using Core.world.map;
    using Hell.enemy;
    using OpenTK.Mathematics;
    using System;
    using System.Timers;

    public class Base_Map : Map {
        private Camera camera;
        private Timer enemySpawnTimer;
        private Random random;

        public Base_Map(Camera camera) {
            this.camera = camera;
            this.random = new Random();

            this.enemySpawnTimer = new Timer(random.Next(1000, 5000));
            this.enemySpawnTimer.Elapsed += SpawnEnemy;
            this.enemySpawnTimer.AutoReset = true;
            this.enemySpawnTimer.Start();
        }

        private void SpawnEnemy(object sender, ElapsedEventArgs e) {
            Vector2 viewSize = camera.Get_View_Size_In_World_Coord();

            int offset_y = random.Next((int)viewSize.Y);
            int offset_x = random.Next((int)viewSize.X);
           
            this.Add_Character(new AIC_simple(new CH_test_enemy()),
                new Vector2(100, 100),
                random.NextSingle() * (float)Math.PI * 2);

            this.enemySpawnTimer.Interval = random.Next(1000, 5000);
        }

    }
}