
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
        public Sprite Sprite { get; set; }
        public float Lifetime { get; set; } = 5f;
        public DateTime CreationTime { get; set; }
        public bool HasHit { get; set; } = false;
        private bool should_destroy = false;

        public Projectile(Vector2 position, Vector2 direction, Vector2 size, float speed = 10f, float damage = 1f, Collision_Shape shape = Collision_Shape.Square) : base(position, size) {

            Console.WriteLine($"Creating projectile => body count: {Game.Instance.get_active_map().physicsWorld.GetBodyCount()}");
            if(Game.Instance == null || Game.Instance.get_active_map() == null || Game.Instance.get_active_map().physicsWorld == null)
                throw new Exception("Game instance, active map, or physics world is not initialized");

            Speed = speed;
            Damage = damage;
            Add_Collider(new Collider(shape, Collision_Type.bullet, null, 1f, direction * speed));
            direction.Normalize();
            collider.velocity = direction * speed;
            Sprite = new Sprite();
            Set_Sprite(Sprite);

            BodyDef def = new();
            def.Position.Set(position.X, position.Y);
            def.AllowSleep = false;
            def.LinearDamping = 0f;

            PolygonDef polygonDef = new();
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
            Console.WriteLine($"               => body count: {Game.Instance.get_active_map().physicsWorld.GetBodyCount()}");
        }

        public override void Update(float deltaTime) {

            if ((DateTime.Now - CreationTime).TotalSeconds > Lifetime || should_destroy) {

                // destroy projectile and body
                Console.WriteLine($"Destrox projectile => body count: {Game.Instance.get_active_map().physicsWorld.GetBodyCount()}");
                if(Game.Instance != null && Game.Instance.get_active_map() != null) {

                    if(collider != null && collider.body != null) {

                        Console.WriteLine($"destroying some more stuff");

                        collider.body.SetUserData(null);
                        Game.Instance.get_active_map().physicsWorld.DestroyBody(collider.body);
                        collider.body = null;
                    }

                    Game.Instance.get_active_map().Remove_Game_Object(this);
                }

                Console.WriteLine($"body count: {Game.Instance.get_active_map().physicsWorld.GetBodyCount()}");
            }

        }

        public void destroy() { should_destroy = true; }

        
        public override void Hit(hitData hit) { }
    }
}
