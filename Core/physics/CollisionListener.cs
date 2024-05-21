
namespace Core.physics {

    using Box2DX.Dynamics;
    using Core.world;
    using OpenTK.Mathematics;

    public class CollisionListener : ContactListener {

        public override void Add(ContactPoint point) {

            Game_Object object1 = point.Shape1.GetBody().GetUserData() as Game_Object;
            Game_Object object2 = point.Shape2.GetBody().GetUserData() as Game_Object;

            if(object1 != null && object2 != null) {

                hitData hit = new hitData();
                hit.is_hit = true;
                hit.hit_force = point.Velocity.Length();
                hit.hit_position = new Vector2(point.Position.X, point.Position.Y);
                hit.hit_direction = new Vector2(point.Velocity.X, point.Velocity.Y);
                hit.hit_normal = new Vector2(point.Normal.X, point.Normal.Y);
                hit.hit_impact_point = hit.hit_position;

                hit.hit_object = object2;
                object1.Hit(hit);

                hit.hit_object = object1;
                object2.Hit(hit);
            }
        }

        public override void Persist(ContactPoint point) {

            Body body1 = point.Shape1.GetBody();
            Body body2 = point.Shape2.GetBody();
        }

        public override void Remove(ContactPoint point) {

            Body body1 = point.Shape1.GetBody();
            Body body2 = point.Shape2.GetBody();
        }

        public override void Result(ContactResult point) {

            Body body1 = point.Shape1.GetBody();
            Body body2 = point.Shape2.GetBody();
        }
    }
}
