using Core.physics;
using Core.render;
using Core.world;
using Box2DX.Common;
using Box2DX.Dynamics;
using OpenTK.Mathematics;
using Core;

namespace Hell.weapon {
    public class Projectile : Game_Object {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public bool Bounce { get; set; }
        public Sprite Sprite { get; set; }
        public Body Body { get; set; }

        public Projectile(Vector2 position, Vector2 direction, float speed = 10f, float damage = 1f, bool bounce = false, Collision_Shape shape = Collision_Shape.Square) : base(position) {
            if (Game.Instance == null || Game.Instance.get_active_map() == null || Game.Instance.get_active_map().physicsWorld == null) {
                throw new Exception("Game instance, active map, or physics world is not initialized");
            }
            Speed = speed;
            Damage = damage;
            Bounce = bounce;
            collider = new Collider(shape, Collision_Type.bullet, null, 1f, direction * speed);
            Sprite = new Sprite();
            Set_Sprite(Sprite);

            BodyDef def = new BodyDef();
            def.Position.Set(position.X, position.Y);
            def.AllowSleep = false;
            Body = Game.Instance.get_active_map().physicsWorld.CreateBody(def);
            Body.IsBullet();
        }

        public override void Update(float deltaTime) {
            Body.SetLinearVelocity(new Vec2(collider.velocity.X, collider.velocity.Y) * Speed * deltaTime);
        }

        public override void Hit(hitData hit) {
            if (Bounce) {
                // Calculate reflection direction
            } else {
                Game.Instance.get_active_map().physicsWorld.DestroyBody(Body);
            }
        }
    }
}