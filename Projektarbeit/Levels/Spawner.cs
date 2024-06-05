namespace Hell.enemy
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
                return this.enemyControllerType;
            }

            set
            {
                if (value.BaseType == typeof(AI_Controller))
                {
                    this.enemyControllerType = value;
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
            this.startTime = Game_Time.total;
            Console.WriteLine("Start time:" + this.startTime);
            this.ControllerType = controllerType;
            this.SpawnRate = rate;
            this.Delay = delay;
            this.MaxSpawn = maxSpawn;
            this.Active = active;
        }

        public override void Update(float delta)
        {
            if (!this.Active)
            {
                return;
            }

            if (this.spawned >= this.MaxSpawn)
            {
                this.Active = false;
                return;
            }

            if (this.Delay > 0)
            {
                this.Delay -= delta;
                return;
            }

            if (Game_Time.total > this.startTime + (this.spawned * this.SpawnRate))
            {
                this.spawned++;
                AI_Controller controller = (AI_Controller)Activator.CreateInstance(this.ControllerType, this.transform.position);
                Game.Instance.get_active_map().add_AI_Controller(controller);
            }
        }

        private readonly float startTime;
    }
}