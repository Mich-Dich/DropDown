
namespace Hell.Levels {

    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Hell.enemy;
    using Hell.player.power;
    using Core.Controllers.ai;

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

            Set_Background_Image("assets/textures/background/background.png", 1.18f);
            all_game_objects.Add(new Spawner(new Vector2(0, -600), typeof(SwarmEnemyController), 10, true, 3, 0));

        }

        public override void update(float deltaTime) {
            /* if((time_stamp + time_interval) <= Game_Time.total) {

                int maxEnemies = 10 + Game.Instance.Score / 10;
                int currentEnemies = Game.Instance.get_active_map().allCharacter.Count;

                if (currentEnemies < maxEnemies) {
                    int enemyType = random.Next(0, 4);
                    Vector2 spawnPosition = new Vector2(random.Next(-250, 250), -600);

                    switch (enemyType) {
                        case 0:
                            add_AI_Controller(new SwarmEnemyController(spawnPosition));
                            break;
                        case 1:
                            add_AI_Controller(new SniperEnemyController(spawnPosition));
                            break;
                        case 2:
                            add_AI_Controller(new SwarmEnemyController(spawnPosition));
                            break;
                        case 3:
                            add_AI_Controller(new TankEnemyController(spawnPosition));
                            break;
                    }
                }

                if (random.NextDouble() < 0.1) {
                    Vector2 powerUpPosition = new Vector2(random.Next(-400, 400), random.Next(-400, 400));

                    if (Game.Instance.player.health < 50 && random.NextDouble() < 0.5) {
                        HealthBoost healthBoost = new HealthBoost(powerUpPosition);
                        Add_Game_Object(healthBoost);
                    } else {
                        switch (random.Next(0, 3)) {
                            case 0:
                                SpeedBoost speedBoost = new SpeedBoost(powerUpPosition);
                                Add_Game_Object(speedBoost);
                                break;
                            case 1:
                                HealthIncrease healthBoost = new HealthIncrease(powerUpPosition);
                                Add_Game_Object(healthBoost);
                                break;
                            case 2:
                                FireRateBoost fireRateBoost = new FireRateBoost(powerUpPosition);
                                Add_Game_Object(fireRateBoost);
                                break;
                        }
                    }
                }

                time_stamp = Game_Time.total;
                time_interval = random.Next(2, 4);
            } */
        }
    }
}
