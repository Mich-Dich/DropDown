
using Core.game_objects;
using Core.physics.material;
using OpenTK.Mathematics;

namespace Core.physics {

    public struct collider {

        public collision_shape  shape { get; set; }    
        public collision_type   type { get; set; }
        public transform        offset { get; set; }
        public physics_material material { get; set; }
        public hit_data         hit_data {  get; set; }

        public float            mass { get; set; }
        public Vector2          velocity { get; set; }

        public collider() {

            this.shape = collision_shape.Square;
            this.type = collision_type.world;
            this.offset = new transform();
            this.material = new physics_material();
        }

        public collider(collision_shape shape = collision_shape.Circle, collision_type type = collision_type.world, transform? offset = null, physics_material? material = null, float mass = 0.0f, Vector2? velocity = null) {

            this.shape = shape;
            this.type = type;
            this.mass = mass;
            this.offset = offset == null? new transform() : offset;
            this.material = material == null ? new physics_material(): material.Value;
            this.velocity = velocity == null ? new Vector2() : velocity.Value;

        }

        public collider(Vector2 velocity, physics_material physics_material) : this() {
            this.velocity = velocity;
            this.material = physics_material;
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
