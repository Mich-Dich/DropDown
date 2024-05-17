namespace Core.physics {
    using Box2DX.Dynamics;
    using Core.world;
    using OpenTK.Mathematics;
    public class EnemyContactListener : ContactListener {
        private Game_Object owner;

        public EnemyContactListener(Game_Object owner) {
            this.owner = owner;
        }

        public override void Add(ContactPoint point) {
            Game_Object objectB = point.Shape2.GetBody().GetUserData() as Game_Object;

            if (objectB != null) {
                Core.physics.hitData hit = new Core.physics.hitData();
                hit.hit_force = point.Velocity.Length();
                hit.hit_position = new Vector2(point.Position.X, point.Position.Y);
                hit.hit_direction = new Vector2(point.Velocity.X, point.Velocity.Y);
                hit.hit_normal = new Vector2(point.Normal.X, point.Normal.Y);
                hit.hit_impact_point = hit.hit_position;
                hit.hit_object = objectB;

                this.owner.Hit(hit);
            }
        }
    }
}