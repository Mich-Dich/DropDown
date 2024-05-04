using Core.util;
using OpenTK.Mathematics;

namespace Core.game_objects {

    public class transform {

        public Vector2      size { get; set; } = new Vector2(100, 100);
        public float        rotation { get; set; } = 0;
        public mobility     mobility { get; set; } = mobility.DYNAMIC;    // conserning update method
        public transform?   parent { get; set; }

        public transform() {

            this.position = new Vector2();
        }

        public transform(transform transform) {

            this.position = transform.position;
            this.size = transform.size;
            this.rotation = transform.rotation;
            this.mobility = transform.mobility;
            this.parent = transform.parent;

        }

        public transform(Vector2? position = null, Vector2? size = null, Single rotation = 0, mobility mobility = mobility.DYNAMIC, transform? parent = null) {

            this._position = position?? new Vector2();
            this.size = size?? new Vector2();
            this.rotation = rotation;
            this.mobility = mobility;
            this.parent = parent;
        }

        public Vector2 position {
            get {
                if(this.parent == null) {
                    return this._position;
                }
                else {
                    return this.parent.position + this._position;
                }
            }

            set {
                this._position = (this.parent == null) ? value : value - this.parent.position;
            }
        }

        public Matrix4 GetTransformationMatrix() {

            Vector2 position = this.position;
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(this.size.X, this.size.Y, 1));
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
            Vector2 velocity = direction * speed * game_time.delta;
            this.position += velocity;
        }

        // ------------------------------------ private ------------------------------------

        private Vector2     _position;

        private Vector2 RotateVector(Vector2 vector, float degrees) {

            float radians = MathHelper.DegreesToRadians(degrees);
            float x = (vector.X * MathF.Cos(radians)) - (vector.Y * MathF.Sin(radians));
            float y = (vector.X * MathF.Sin(radians)) + (vector.Y * MathF.Cos(radians));
            return new Vector2(x, y);
        }
    }
}