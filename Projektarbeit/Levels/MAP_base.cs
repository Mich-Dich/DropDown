namespace Hell.Levels
{
    using Core.util;
    using Core.world;
    using Hell.enemy;
    using Hell.player.power;
    using OpenTK.Mathematics;

    internal class MAP_base : Map
    {
        private readonly Camera camera;
        private readonly Random random;
        private float timeStamp;
        private float timeInterval;

        public MAP_base()
        {
            this.use_garbage_collector = true;
            this.camera = Game.Instance.camera;
            this.random = new Random();

            this.timeStamp = Game_Time.total;
            this.timeInterval = this.random.Next(2, 4);

            this.Set_Background_Image("assets/textures/background/background.png", 1.18f);
        }

        public override void update(float deltaTime)
        {
            if ((this.timeStamp + this.timeInterval) <= Game_Time.total)
            {
                int maxEnemies = 10 + (Game.Instance.Score / 10);
                int currentEnemies = Game.Instance.get_active_map().allCharacter.Count;

                if (currentEnemies < maxEnemies)
                {
                    int enemyType = this.random.Next(0, 4);
                    Vector2 spawnPosition = new (this.random.Next(-250, 250), -600);

                    switch (enemyType)
                    {
                        case 0:
                            this.add_AI_Controller(new SwarmEnemyController(spawnPosition));
                            break;
                        case 1:
                            this.add_AI_Controller(new SniperEnemyController(spawnPosition));
                            break;
                        case 2:
                            this.add_AI_Controller(new SwarmEnemyController(spawnPosition));
                            break;
                        case 3:
                            this.add_AI_Controller(new TankEnemyController(spawnPosition));
                            break;
                    }
                }

                if (this.random.NextDouble() < 0.1)
                {
                    Vector2 powerUpPosition = new (this.random.Next(-400, 400), this.random.Next(-400, 400));

                    if (Game.Instance.player.health < 50 && this.random.NextDouble() < 0.5)
                    {
                        HealthBoost healthBoost = new (powerUpPosition);
                        this.Add_Game_Object(healthBoost);
                    }
                    else
                    {
                        switch (this.random.Next(0, 3))
                        {
                            case 0:
                                SpeedBoost speedBoost = new (powerUpPosition);
                                this.Add_Game_Object(speedBoost);
                                break;
                            case 1:
                                HealthIncrease healthBoost = new (powerUpPosition);
                                this.Add_Game_Object(healthBoost);
                                break;
                            case 2:
                                FireRateBoost fireRateBoost = new (powerUpPosition);
                                this.Add_Game_Object(fireRateBoost);
                                break;
                        }
                    }
                }

                this.timeStamp = Game_Time.total;
                this.timeInterval = this.random.Next(2, 4);
            }
        }
    }
}