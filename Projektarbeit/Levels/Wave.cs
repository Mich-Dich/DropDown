namespace Projektarbeit.Levels
{
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.controller;

    public class Wave : Game_Object
    {
        public Wave(List<Spawner> spawners)
        :base(mobility: Mobility.STATIC)
        {
            this.spawners = spawners;
        }
     
        private bool Started = false;
        private bool Finished = false;
        private static List<Wave> waves;
        public static int currentWave {get; private set;} = 0;
        
        private readonly List<Spawner> spawners;

        public void InitializeWave()
        {
            // Activate spawners
            foreach (Spawner spawner in spawners)
            {
                spawner.Active = true;
            }
            Started = true;
            Game.Instance.get_active_map().all_game_objects.Add(this);
        }

        public void RemoveWave()
        {
            Game.Instance.get_active_map().all_game_objects.Remove(this);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (Finished)
            {
                if (Game.Instance.get_active_map().allCharacter.Count < 2) NextWave();
                return;
            }

            foreach (Spawner spawner in spawners)
            {
                spawner.Update(deltaTime);
            }

            if (Started && !Finished)
            {
                if (spawners.TrueForAll(spawner => spawner.Active == false))
                {
                    Finished = true;
                }
            }
        }

        public static void NextWave()
        {
            waves[currentWave].RemoveWave();

            if (currentWave < waves.Count - 1)
            {
                currentWave++;
                waves[currentWave].InitializeWave();
            }
            else
            {
                Console.WriteLine("Game Over");
                // End game
            }
            
        }

        public static void LoadWaves()
        {
            /*
            10 waves with last as a boss wave 
            Possible enemies in ascending order of difficulty:
            - SwarmEnemyController
            - TankEnemyController
            - SniperEnemyController
            - ExplosivEnemyController
            - BossController
            */
            currentWave = 0;

            waves = new List<Wave>(
                [
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 3),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 9, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 1, rate: 12, delay: 7),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 1, rate: 18, delay: 7),
                        new Spawner(new Vector2(-400, -600), typeof(SniperEnemyController), maxSpawn: 1, rate: 12, delay: 17),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 2, rate: 18, delay: 7),
                        new Spawner(new Vector2(400, -600), typeof(SniperEnemyController), maxSpawn: 1, rate: 16, delay: 17),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 2, rate: 18, delay: 7),
                        new Spawner(new Vector2(-400, -600), typeof(SniperEnemyController), maxSpawn: 2, rate: 16, delay: 17),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 3, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 2, rate: 18, delay: 7),
                        new Spawner(new Vector2(400, -600), typeof(SniperEnemyController), maxSpawn: 2, rate: 16, delay: 17),
                        new Spawner(new Vector2(-700, -600), typeof(ExplosivEnemyController), maxSpawn: 1, rate: 12, delay: 25),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 2, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 3, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 2, rate: 18, delay: 7),
                        new Spawner(new Vector2(-400, -600), typeof(SniperEnemyController), maxSpawn: 2, rate: 16, delay: 17),
                        new Spawner(new Vector2(-700, -600), typeof(ExplosivEnemyController), maxSpawn: 2, rate: 12, delay: 20),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 3, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 3, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 2, rate: 18, delay: 7),
                        new Spawner(new Vector2(400, -600), typeof(SniperEnemyController), maxSpawn: 2, rate: 16, delay: 17),
                        new Spawner(new Vector2(-250, -600), typeof(ExplosivEnemyController), maxSpawn: 2, rate: 12, delay: 20),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(700, -600), typeof(SwarmEnemyController), maxSpawn: 3, rate: 8, delay: 0),
                        new Spawner(new Vector2(-700, -600), typeof(SwarmEnemyController), maxSpawn: 3, rate: 10, delay: 3),
                        new Spawner(new Vector2(0, -600), typeof(TankEnemyController), maxSpawn: 3, rate: 18, delay: 7),
                        new Spawner(new Vector2(400, -600), typeof(SniperEnemyController), maxSpawn: 2, rate: 16, delay: 17),
                        new Spawner(new Vector2(-250, -600), typeof(ExplosivEnemyController), maxSpawn: 3, rate: 12, delay: 20),
                    ]),
                    new Wave(
                    [
                        new Spawner(new Vector2(0, -600), typeof(BossController), maxSpawn: 1, rate: 20, delay: 0),
                        new Spawner(new Vector2(300, -600), typeof(SwarmEnemyController), maxSpawn: 1, rate: 8, delay: 3),
                        new Spawner(new Vector2(-300, -600), typeof(SwarmEnemyController), maxSpawn: 1, rate: 8, delay: 3),
                        new Spawner(new Vector2(500, -600), typeof(SniperEnemyController), maxSpawn: 1, rate: 12, delay: 8),
                        new Spawner(new Vector2(-500, -600), typeof(SniperEnemyController), maxSpawn: 1, rate: 12, delay: 10),
                    ])
                ]
            );
            waves[0].InitializeWave();
        }
    }
}