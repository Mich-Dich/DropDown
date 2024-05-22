namespace Core.defaults {
    using Core.physics;
    using Core.world;
    using OpenTK.Mathematics;
    using Core.render;
    using Box2DX.Collision;
    using Box2DX.Common;
    using Box2DX.Dynamics;

    public abstract class PowerUp : Game_Object {

        public float Duration { get; set; } = 5f;
        public DateTime ActivationTime { get; set; }

        public PowerUp(Vector2 position, Vector2 size, Sprite sprite) : base(position, size) {
            if(Game.Instance == null || Game.Instance.get_active_map() == null || Game.Instance.get_active_map().physicsWorld == null)
                throw new Exception("Game instance, active map, or physics world is not initialized");
            
            Set_Sprite(sprite);

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
            if((DateTime.Now - ActivationTime).TotalSeconds > Duration) {
                Deactivate(Game.Instance.player);
                Game.Instance.get_active_map().Remove_Game_Object(this);
            }
        }

        public abstract void Activate(Game_Object target);
        public abstract void Deactivate(Game_Object target);


        public override void Hit(hitData hit) {
            if(hit.hit_object == Game.Instance.player) {
                Activate(Game.Instance.player);
                Game.Instance.get_active_map().Remove_Game_Object(this);
            }
        }
    }
}