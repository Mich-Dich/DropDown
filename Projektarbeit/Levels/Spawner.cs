using Core.Controllers.ai;
using Core.util;
using Core.world;
using OpenTK.Mathematics;

namespace Hell.enemy {

    public class Spawner : Game_Object {

        public bool Active { get; set; }
        public float SpawnRate { get; set; }
        public float Delay { get; set; }
        public int MaxSpawn { get; set; }
        public Type ControllerType 
        {
            get { return _enemyControllerType; }
            set { 
                if(value.BaseType == typeof(AI_Controller)) {
                    _enemyControllerType = value;
                } else {
                    throw new Exception("Invalid Type");
                }
            }
        }
        private Type _enemyControllerType;
        private int _spawned = 0;
        private float _startTime;
        public Spawner(Vector2 position, Type controllerType, int maxSpawn, bool active = false, float rate = 5, float delay = 0) : base(position, new Vector2(10, 10), 0, Mobility.STATIC) {
            
            _startTime = Game_Time.total;
            Console.WriteLine("Start time:" + _startTime);
            ControllerType = controllerType;
            SpawnRate = rate;
            Delay = delay;
            MaxSpawn = maxSpawn;
            Active = active;

        }

        public override void Update(float delta) {

            if(!Active) return;
            if(_spawned >= MaxSpawn) {
                Active = false;
                return;
            }
            if(Delay > 0) {
                Delay -= delta;
                return;
            }
            if(Game_Time.total > _startTime + _spawned * SpawnRate) {
                this._spawned++;
                AI_Controller controller = (AI_Controller) Activator.CreateInstance(ControllerType, [transform.position]);
                Game.Instance.get_active_map().add_AI_Controller(controller);
            }
        }
    }
}