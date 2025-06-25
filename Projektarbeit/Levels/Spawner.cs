namespace Projektarbeit.Levels
{
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;
    using Projektarbeit.characters.enemy.controller;

    public class Spawner : Game_Object
    {
        public bool Active { get; set; }

        public float SpawnRate { get; }

        public float StartDelay { get; set; }

        public int MaxSpawn { get; set; }

        public Type ControllerType
        {
            get
            {
                return enemyControllerType;
            }

            set
            {
                if (value.BaseType == typeof(AI_Controller))
                {
                    enemyControllerType = value;
                }
                else
                {
                    throw new Exception("Invalid Type");
                }
            }
        }

        private readonly float startTime;
        private Type enemyControllerType;
        protected int spawned = 0;
        private float lastSpawnTime = 0;

        public Spawner(Vector2 position, Type controllerType, int maxSpawn, float rate = 5, float delay = 0, bool active = false)
            : base(position, new Vector2(10, 10), 0, Mobility.STATIC)
        {
            startTime = Game_Time.total;
            Console.WriteLine("Spawner start time:" + startTime);
            ControllerType = controllerType;
            SpawnRate = rate; //Rate of spawn in seconds
            StartDelay = delay; //Delay before first spawn
            MaxSpawn = maxSpawn;
            Active = active;
        }

        public override void Update(float delta)
        {
            if (!Active)
            {
                return;
            }

            if (StartDelay > 0)
            {
                StartDelay -= delta;
                return;
            }

            if (spawned >= MaxSpawn)
            {
                if (Active) // Only log once when becoming inactive
                {
                    Console.WriteLine($"[Spawner] {ControllerType.Name} spawner finished: {spawned}/{MaxSpawn} enemies spawned");
                }
                Active = false;
                return;
            }

            // More reliable spawning logic using delta time accumulation
            if (lastSpawnTime == 0) // First spawn after delay
            {
                lastSpawnTime = Game_Time.total;
            }
            
            if (Game_Time.total >= lastSpawnTime + SpawnRate)
            {
                lastSpawnTime = Game_Time.total;
                Console.WriteLine($"[Spawner] {ControllerType.Name} spawning enemy {spawned + 1}/{MaxSpawn} at time {Game_Time.total:F1}");
                
                // Add extra debugging for boss spawners
                if (ControllerType == typeof(BossController))
                {
                    Console.WriteLine($"[Spawner] BOSS SPAWN DEBUG: Spawner at {transform.position}, MaxSpawn={MaxSpawn}, Spawned={spawned}");
                }
                
                try
                {
                    AI_Controller controller = (AI_Controller)Activator.CreateInstance(ControllerType, transform.position);
                    Core.Game.Instance.get_active_map().add_AI_Controller(controller);
                    Console.WriteLine($"[Spawner] SUCCESS: {ControllerType.Name} enemy {spawned + 1} created and added to map");
                    
                    // Add extra debugging for boss controllers
                    if (ControllerType == typeof(BossController))
                    {
                        Console.WriteLine($"[Spawner] BOSS CONTROLLER CREATED: {controller.GetType().Name} at {transform.position}");
                    }
                    spawned++; // Only increment once after successful spawn
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Spawner] ERROR spawning {ControllerType.Name}: {ex}");
                    // Do NOT decrement spawned on error, to avoid infinite retries
                }
            }
        }
    }

    public class BossSpawner : Spawner
    {
        private bool hasSpawned = false;

        public BossSpawner(Vector2 position, Type controllerType, int maxSpawn, float rate = 0, float delay = 0, bool active = false)
            : base(position, controllerType, maxSpawn, rate, delay, active)
        {
        }

        public override void Update(float delta)
        {
            if (!Active || hasSpawned)
            {
                return;
            }

            if (StartDelay > 0)
            {
                StartDelay -= delta;
                return;
            }

            // Spawn the boss immediately and mark as spawned
            Console.WriteLine($"[BossSpawner] Spawning boss at {transform.position}");
            try
            {
                AI_Controller controller = (AI_Controller)Activator.CreateInstance(ControllerType, transform.position);
                Core.Game.Instance.get_active_map().add_AI_Controller(controller);
                Console.WriteLine($"[BossSpawner] SUCCESS: Boss created and added to map");
                spawned++;
                hasSpawned = true;
                Active = false; // Immediately deactivate after spawning
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BossSpawner] ERROR spawning boss: {ex}");
                // Don't retry on error, just deactivate
                hasSpawned = true;
                Active = false;
            }
        }
    }
}