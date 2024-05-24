
namespace Core.defaults {

    using Box2DX.Collision;
    using Box2DX.Dynamics;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public abstract class PowerUp : Game_Object {

        private float live_time { get; set; } = 0f;     // 0f: infinit live_time
        public float Duration { get; set; } = 5f;
        public float ActivationTime { get; set; }

        public Action<Character> activation { get; set; }
        public Action<Character> deactivation { get; set; }
        public Action destruction { get; set; }             // an action that is called befor destruction on the same tread

        public PowerUp(Vector2 position, Vector2 size, Sprite sprite) : base(position, size) {

            Set_Sprite(sprite);

            Game.Instance.get_active_map().Add_Game_Object(this);

            BodyDef def = new BodyDef();
            def.Position.Set(position.X, position.Y);
            def.AllowSleep = false;
            def.LinearDamping = 0f;

            PolygonDef polygonDef = new ();
            polygonDef.SetAsBox(size.X / 2, size.Y / 2);
            polygonDef.Density = 1f;
            polygonDef.Friction = 0.3f;
            polygonDef.IsSensor = true;

            // Initialize the collider
            Add_Collider(new Collider(Collision_Shape.Circle, Collision_Type.bullet, null, 1f));

            collider.body = Game.Instance.get_active_map().physicsWorld.CreateBody(def);
            collider.body.CreateShape(polygonDef);
            collider.body.SetMassFromShapes();
            collider.body.SetUserData(this);
        }

        public override void Update(float deltaTime) {

            if(live_time > 0f) {
                if(Game_Time.total >= ActivationTime + Duration) {
                
                    Console.WriteLine($"destruction");
                    destruction();
                    Game.Instance.get_active_map().Remove_Game_Object(this);
                }
            }
        }

        public override void Hit(hitData hit) {
            if(hit.hit_object == Game.Instance.player) {
                Game.Instance.player.add_power_up(this);
                ActivationTime = Game_Time.total;
                Game.Instance.get_active_map().Remove_Game_Object(this);
            }
        }
    }

}