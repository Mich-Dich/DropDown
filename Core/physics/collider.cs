
using Core.game_objects;

namespace Core.physics {

    public struct collider {

        public collision_shape shape { get; set; }    
        public collision_type type { get; set; }
        public transform offset { get; set; }

        public collider() {

            this.shape = collision_shape.Square;
            this.type = collision_type.world;
            this.offset = new transform();
        }

        public collider(collision_shape shape, collision_type type, transform offset) {
            this.shape = shape;
            this.type = type;
            this.offset = offset;
        }
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
