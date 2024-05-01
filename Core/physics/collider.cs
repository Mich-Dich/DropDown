
using Core.game_objects;
using Core.physics.material;

namespace Core.physics {

    public struct collider {

        public collision_shape shape { get; set; }    
        public collision_type type { get; set; }
        public transform offset { get; set; }
        public physics_material material { get; set; }

        public collider() {

            this.shape = collision_shape.Square;
            this.type = collision_type.world;
            this.offset = new transform();
            this.material = new physics_material();
        }

        public collider(collision_shape shape = collision_shape.Circle, collision_type type = collision_type.world, transform? offset = null, physics_material? material = null) {

            this.shape = shape;
            this.type = type;
            if (offset == null) 
                this.offset = new transform();
            else 
                this.offset = offset;

            if (material == null)
                this.material = new physics_material();
            else
                this.material = material.Value;
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
