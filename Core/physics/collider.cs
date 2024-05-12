
namespace Core.physics {

    using Core.util;
    using OpenTK.Mathematics;

    public sealed class Collider {

        public collision_shape  shape;
        public collision_type   type;
        public Transform        offset;
        public physics_material material;
        public hit_data         hit_data;

        public float            mass;
        public Vector2          velocity {  get; set; }
        public bool Blocking { get; set; } = true;

        public Collider() {

            this.shape = collision_shape.Square;
            this.type = collision_type.world;
            this.offset = new Transform();
            this.material = new physics_material();
        }

        public Collider(collision_shape shape = collision_shape.Circle, collision_type type = collision_type.world, Transform? offset = null, physics_material? material = null, float mass = 100.0f, Vector2? velocity = null) {

            this.shape = shape;
            this.type = type;
            this.mass = mass;
            this.offset = offset == null? new Transform(Vector2.Zero, Vector2.Zero) : offset;
            this.material = material == null ? new physics_material(): material.Value;
            this.velocity = velocity == null ? new Vector2() : velocity.Value;
        }

        public Collider set_offset(Transform offset) {

            this.offset = offset;
            return this;
        }

        public Collider set_mass(float mass) {

            this.mass = mass;
            return this;
        }

        public Collider set_physics_material(physics_material material) {

            this.material = material;
            return this;
        }

        //public collider(Vector2 velocity, physics_material physics_material) : this() {
        //    this.velocity = velocity;
        //    this.material = physics_material;
        //}
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
