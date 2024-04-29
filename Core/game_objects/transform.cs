using Core.util;
using OpenTK.Mathematics;

namespace Core.game_objects {

    public class transform {

        public Vector2      scale { get; set; }
        public float        rotation { get; set; }
        public transform?   parent { get; set; }

        public Vector2 Position {
            get {
                if(this.parent == null) {
                    return this.position;
                }
                else {
                    return this.parent.Position + this.position;
                }
            }

            set {
                this.position = this.parent == null ? value : value - this.parent.Position;
            }
        }

        public Matrix4 GetTransformationMatrix() {

            Vector2 position = this.Position;
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(this.scale.X, this.scale.Y, 1));
            Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.rotation));

            return scale * rotation * translation;
        }

        public Vector2 TransformPoint(Vector2 point) {

            Vector4 homogenousPoint = new Vector4(point.X, point.Y, 0, 1);
            Vector4 transformedPoint = this.GetTransformationMatrix() * homogenousPoint;
            return new Vector2(transformedPoint.X, transformedPoint.Y);
        }

        public Vector2 InverseTransformPoint(Vector2 point) {

            Matrix4 inverseTransform = Matrix4.Invert(this.GetTransformationMatrix());
            Vector4 homogenousPoint = new Vector4(point.X, point.Y, 0, 1);
            Vector4 transformedPoint = inverseTransform * homogenousPoint;
            return new Vector2(transformedPoint.X, transformedPoint.Y);
        }

        public void Move(Vector2 direction, float speed) {

            direction = this.RotateVector(direction, this.rotation);
            Vector2 velocity = direction * speed * (float)game_time.elapsed.Seconds;
            this.Position += velocity;
        }

        // ------------------------------------ private ------------------------------------

        private Vector2     position;

        private Vector2 RotateVector(Vector2 vector, float degrees) {

            float radians = MathHelper.DegreesToRadians(degrees);
            float x = (vector.X * MathF.Cos(radians)) - (vector.Y * MathF.Sin(radians));
            float y = (vector.X * MathF.Sin(radians)) + (vector.Y * MathF.Cos(radians));
            return new Vector2(x, y);
        }
    }
}