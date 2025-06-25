namespace Projektarbeit.Levels
{
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class Spawner : Game_Object
    {
        public bool Active { get; set; }

        public float SpawnRate { get; }

        public float StartDelay { get; private set;}

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
        private int spawned = 0;
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
                spawned++;
                lastSpawnTime = Game_Time.total;
                Console.WriteLine($"[Spawner] {ControllerType.Name} spawning enemy {spawned}/{MaxSpawn} at time {Game_Time.total:F1}");
                try
                {
                    AI_Controller controller = (AI_Controller)Activator.CreateInstance(ControllerType, transform.position);
                    Core.Game.Instance.get_active_map().add_AI_Controller(controller);
                    Console.WriteLine($"[Spawner] SUCCESS: {ControllerType.Name} enemy {spawned} created and added to map");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Spawner] ERROR spawning {ControllerType.Name}: {ex.Message}");
                    // Decrement spawn count if enemy creation failed
                    spawned--;
                    Console.WriteLine($"[Spawner] Decremented spawn count due to error: {spawned}/{MaxSpawn}");
                }
            }
        }

    }
}