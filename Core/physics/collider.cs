namespace Core.physics
{
    using Box2DX.Dynamics;
    using Core.util;
    using OpenTK.Mathematics;

    public sealed class Collider
    {
        public Collision_Shape shape;
        public Collision_Type type;
        public Transform offset;
        public Physics_Material material;
        public hitData hitData;

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

        public Collider(Body body)
        {
            this.body = body;
            this.offset = new Transform(Vector2.Zero, Vector2.Zero);
        }

        public Collider()
        {
            this.shape = Collision_Shape.Square;
            this.type = Collision_Type.world;
            this.offset = new Transform();
            this.material = default(Physics_Material);
        }

        public Collider(Collision_Shape shape = Collision_Shape.Circle, Collision_Type type = Collision_Type.world, Transform? offset = null, Physics_Material? material = null, float mass = 100.0f, Vector2? velocity = null)
        {
            this.shape = shape;
            this.type = type;
            this.mass = mass;
            this.offset = offset == null ? new Transform(Vector2.Zero, Vector2.Zero) : offset;
            this.material = material == null ? default(Physics_Material) : material.Value;
            this.velocity = velocity == null ? default(Vector2) : velocity.Value;
        }

        public Collider Set_Offset(Transform offset)
        {
            this.offset = offset;
            return this;
        }

        public Collider Set_Mass(float mass)
        {
            this.mass = mass;
            return this;
        }

        public Collider Set_Physics_Material(Physics_Material material)
        {
            this.material = material;
            return this;
        }

        // public collider(Vector2 velocity, Physics_Material Physics_Material) : this() {
        //    this.velocity = velocity;
        //    this.material = Physics_Material;
        // }
    }

    public enum Collision_Shape
    {
        None = 0,
        Circle = 1,
        Square = 2,
    }

    public enum Collision_Type
    {
        None = 0,
        world = 1,
        character = 2,
        bullet = 3,
    }
}
