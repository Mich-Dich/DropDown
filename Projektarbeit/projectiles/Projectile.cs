using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Core;
using Core.physics;
using Core.render;
using Core.world;
using OpenTK.Mathematics;
using Core;
using System;

namespace Hell.weapon {
    public class Projectile : Game_Object {

        public float Speed { get; set; }
        public float Damage { get; set; }
        public bool Bounce { get; set; }
        public Sprite Sprite { get; set; }

        public Projectile(Vector2 position, Vector2 direction, float speed = 10f, float damage = 1f, bool bounce = false, Collision_Shape shape = Collision_Shape.Square) : base(position) {

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
            //collider.body.IsBullet();
            collider.body.SetUserData(this);

            collider.body.ApplyForce(new Vec2(collider.velocity.X, collider.velocity.Y) * Speed, collider.body.GetWorldCenter());
        }

        public override void Update(float deltaTime) {
            Vector2 direction = new Vector2(collider.velocity.X, collider.velocity.Y);
            float angle = MathF.Atan2(-direction.X, direction.Y);

            angle = angle % (2 * MathF.PI);

            transform.rotation = MathHelper.RadiansToDegrees(angle);

            Console.WriteLine($"pos: {collider.body.GetPosition().X}, {collider.body.GetPosition().Y}");
        }

        public override void Hit(hitData hit) {
            if(Bounce) {
                // Calculate reflection direction
            }
            else {
                Game.Instance.get_active_map().physicsWorld.DestroyBody(collider.body);
            }
        }
    }
}
