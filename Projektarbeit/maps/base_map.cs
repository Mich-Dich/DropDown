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
            try {
                Vector2 viewSize = camera.Get_View_Size_In_World_Coord();

                int offset_y = random.Next((int)viewSize.Y);
                int offset_x = random.Next((int)viewSize.X);
            
                Vector2 position = new Vector2(100, 100);
                Console.WriteLine($"Creating enemy at position: {position}");
                this.Add_Character(new AIC_simple(new CH_test_enemy()),
                    position,
                    random.NextSingle() * (float)Math.PI * 2);

                this.enemySpawnTimer.Interval = random.Next(1000, 5000);
            } catch (Exception ex) {
                Console.WriteLine($"An error occurred in SpawnEnemy: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

    }
}