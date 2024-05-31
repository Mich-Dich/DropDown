
namespace Core.defaults {

    using Box2DX.Collision;
    using Box2DX.Common;
    using Box2DX.Dynamics;
    using Core.physics;
    using Core.world;
    using OpenTK.Mathematics;

    public abstract class Projectile : Game_Object {

        public float Speed { get; set; }
        public float Damage { get; set; }
        public bool Bounce { get; set; }
        public Sprite Sprite { get; set; }
        public float Lifetime { get; set; } = 5f;
        public DateTime CreationTime { get; set; }
        public bool HasHit { get; set; } = false;

        public Projectile(Vector2 position, Vector2 direction, Vector2 size, float speed = 10f, float damage = 1f, bool bounce = false, Collision_Shape shape = Collision_Shape.Square) : base(position, size) {

            if(Game.Instance == null || Game.Instance.get_active_map() == null || Game.Instance.get_active_map().physicsWorld == null)
                throw new Exception("Game instance, active map, or physics world is not initialized");

            Speed = speed;
            Damage = damage;
            Bounce = bounce;
            Add_Collider(new Collider(shape, Collision_Type.bullet, null, 1f, direction * speed));
            direction.Normalize();
            collider.velocity = direction * speed;
            Sprite = new Sprite();
            Set_Sprite(Sprite);

            BodyDef def = new BodyDef();
            def.Position.Set(position.X, position.Y);
            def.AllowSleep = false;
            def.LinearDamping = 0f;

            PolygonDef polygonDef = new ();
            polygonDef.SetAsBox(transform.size.X / 2, transform.size.Y / 2);
            polygonDef.Density = 1f;
            polygonDef.Density = 1f;
            polygonDef.Friction = 0.3f;
            polygonDef.IsSensor = true;

            collider.body = Game.Instance.get_active_map().physicsWorld.CreateBody(def);
            collider.body.CreateShape(polygonDef);
            collider.body.SetMassFromShapes();
            collider.body.IsDynamic();
            collider.body.IsBullet();
            collider.body.SetUserData(this);

            collider.body.ApplyForce(new Vec2(collider.velocity.X, collider.velocity.Y) * Speed, collider.body.GetWorldCenter());

            rotate_to_vector(direction);
            CreationTime = DateTime.Now;
        }

        public override void Update(float deltaTime) {
            if ((DateTime.Now - CreationTime).TotalSeconds > Lifetime) {
                if (Game.Instance != null && Game.Instance.get_active_map() != null) {
                    Game.Instance.get_active_map().Remove_Game_Object(this); 

                    if (collider != null && collider.body != null) {
                        var world = Game.Instance.get_active_map().physicsWorld;
                        world.DestroyBody(collider.body);
                        collider.body = null;
                    }
                }
            }
        }

        public override void Hit(hitData hit) { }
    }
}
