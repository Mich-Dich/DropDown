namespace Hell {
    using Core.world;
    using Core.world.map;
    using Hell.enemy;
    using OpenTK.Mathematics;
    using System;
    using System.Timers;

    public class Base_Map : Map {
        private Camera camera;

        public Base_Map(Camera camera) {
            this.camera = camera;
        }

        private void SpawnEnemy(object sender, ElapsedEventArgs e) {
            try {
                this.Add_Character(new AIC_simple(new CH_small_bug()), new Vector2(10, 50), 0);

            } catch (Exception ex) {
                Console.WriteLine($"An error occurred in SpawnEnemy: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}