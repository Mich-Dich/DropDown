
using Core.game_objects;

namespace Core.physics {

    public struct collider {
    
        public collision_shape collision_shape {  get; set; }
        public collision_type collision_type { get; set; }
        public transform offset { get; set; }
    }

    public enum collision_shape {

        None = 0,
        Circle = 1,
        Square = 2,
    }

    public enum collision_type {

        None = 0,
        world = 1,
        character = 2,
        bullet = 3,
    }

}
