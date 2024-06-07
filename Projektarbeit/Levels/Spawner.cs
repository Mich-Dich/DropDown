namespace Projektarbeit.Levels
{
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class Spawner : Game_Object
    {
        public bool Active { get; set; }

        public float SpawnRate { get; set; }

        public float Delay { get; set; }

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

        private Type enemyControllerType;
        private int spawned = 0;

        public Spawner(Vector2 position, Type controllerType, int maxSpawn, bool active = false, float rate = 5, float delay = 0)
            : base(position, new Vector2(10, 10), 0, Mobility.STATIC)
        {
            startTime = Game_Time.total;
            Console.WriteLine("Start time:" + startTime);
            ControllerType = controllerType;
            SpawnRate = rate;
            Delay = delay;
            MaxSpawn = maxSpawn;
            Active = active;
        }

        public override void Update(float delta)
        {
            if (!Active)
            {
                return;
            }

            if (spawned >= MaxSpawn)
            {
                Active = false;
                return;
            }

            if (Delay > 0)
            {
                Delay -= delta;
                return;
            }

            if (Game_Time.total > startTime + (spawned * SpawnRate))
            {
                spawned++;
                AI_Controller controller = (AI_Controller)Activator.CreateInstance(ControllerType, transform.position);
                Core.Game.Instance.get_active_map().add_AI_Controller(controller);
            }
        }

        private readonly float startTime;
    }
}