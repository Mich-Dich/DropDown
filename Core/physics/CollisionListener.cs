
using Box2DX.Dynamics;
using Core.world;
using OpenTK.Mathematics;

namespace Core.physics
{
    public class CollisionListener : ContactListener
    {

        public override void Add(ContactPoint point)
        {
            try
            {
                Game_Object? object1 = point.Shape1?.GetBody()?.GetUserData() as Game_Object;
                Game_Object? object2 = point.Shape2?.GetBody()?.GetUserData() as Game_Object;

                if (object1?.collider != null && object2?.collider != null)
                {
                    var hit = new hitData
                    {
                        is_hit = true,
                        hit_force = point.Velocity.Length(),
                        hit_position = new Vector2(point.Position.X, point.Position.Y),
                        hit_direction = new Vector2(point.Velocity.X, point.Velocity.Y),
                        hit_normal = new Vector2(point.Normal.X, point.Normal.Y),
                        hit_impact_point = new Vector2(point.Position.X, point.Position.Y)
                    };

                    hit.hit_object = object2;
                    object1.Hit(hit); // This is where your null reference likely occurs

                    hit.hit_object = object1;
                    object2.Hit(hit);
                }
            }
            catch (NullReferenceException ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error in CollisionListener: {ex.Message}");
            }
        }

        public override void Persist(ContactPoint point)
        {

            Body body1 = point.Shape1.GetBody();
            Body body2 = point.Shape2.GetBody();
        }

        public override void Remove(ContactPoint point)
        {

            Body body1 = point.Shape1.GetBody();
            Body body2 = point.Shape2.GetBody();
        }

        public override void Result(ContactResult point)
        {

            Body body1 = point.Shape1.GetBody();
            Body body2 = point.Shape2.GetBody();
        }
    }
}
