
namespace Core.physics {

    using Box2DX.Dynamics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public sealed class Collider {

        public Collision_Shape  shape;
        public Collision_Type   type;
        public Transform        offset;

        public Body body { get; set; }
        public float mass;
        public Vector2 velocity { get; set; }
        public bool blocking { get; set; } = true;

        /* Unmerged change from project 'Core (net8.0)'
        Before:
                public Collider(Body body)
                {
                    this.body = body;
                    this.offset = new Transform(Vector2.Zero, Vector2.Zero);
                }
        After:
                public Collider(Body body)
                {
                    this.body = body;
                    this.offset = new Transform(Vector2.Zero, Vector2.Zero);
                }
        */

        public Collider(Body body) {
            
            this.body = body;
            this.offset = new Transform(Vector2.Zero, Vector2.Zero);
        }

        public Collider() {

            this.shape = Collision_Shape.Square;
            this.type = Collision_Type.world;
            this.offset = new Transform();
        }

        public Collider(Collision_Shape shape = Collision_Shape.Circle, Collision_Type type = Collision_Type.world, Transform? offset = null, float mass = 100.0f, Vector2? velocity = null) {

            this.shape = shape;
            this.type = type;
            this.mass = mass;
            this.offset = offset == null ? new Transform(Vector2.Zero, Vector2.Zero) : offset;
            this.velocity = velocity == null ? new Vector2() : velocity.Value;
        }

        public Collider Set_Offset(Transform offset) {
            
            this.offset = offset;
            return this;
        }

        public Collider Set_Mass(float mass) {
        
            this.mass = mass;
            return this;
        }

    }

    public enum Collision_Shape {
        
        None = 0,
        Circle = 1,
        Square = 2,
    }

    public enum Collision_Type {
        
        None = 0,
        world = 1,
        character = 2,
        bullet = 3,
    }

    public struct hitData {

        public bool is_hit;
        public float hit_force;
        public Vector2 hit_position;
        public Vector2 hit_direction;
        public Vector2 hit_normal;
        public Vector2 hit_impact_point;
        public Game_Object hit_object;

        public hitData(Single hit_force) : this() {

            this.hit_force = hit_force;
        }
        public hitData(Boolean is_hit, Single hit_force, Vector2 hit_position, Vector2 hit_direction, Vector2 hit_normal, Vector2 hit_impact_point) : this() {

            this.is_hit = is_hit;
            this.hit_force = hit_force;
            this.hit_position = hit_position;
            this.hit_direction = hit_direction;
            this.hit_normal = hit_normal;
            this.hit_impact_point = hit_impact_point;
        }
    }
}
