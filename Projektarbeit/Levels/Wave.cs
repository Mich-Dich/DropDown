namespace Projektarbeit.Levels
{
    using Core.world;

    public class Wave : Game_Object
    {
        public Wave(List<Spawner> spawners)
        :base(mobility: Mobility.STATIC)
        {
            this.spawners = spawners;
        }
     
        private static List<Wave> waves;
        public static int currentWave {get; private set;} = 0;
        
        private readonly List<Spawner> spawners;

        public void InitializeWave()
        {
            // Add spawners to game object list
            // Activate spawners
            foreach (Spawner spawner in spawners)
            {
                Game.Instance.get_active_map().all_game_objects.Add(spawner);
                spawner.Active = true;
            }
        }

        public static void LoadWaves()
        {
            return;
            // Load waves from file
        }
    }
}