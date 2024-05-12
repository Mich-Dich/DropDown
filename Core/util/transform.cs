
namespace Core.util {

    using Core.world;
    using OpenTK.Mathematics;

    public class Transform {

        public Vector2 size { get; set; } = new Vector2(100, 100);
        public float rotation { get; set; } = 0;
        public Mobility mobility { get; set; } = Mobility.DYNAMIC;    // conserning update method
        public Transform? parent { get; set; }

        /// <summary>
        /// Constructs a new Transform with default values (position at origin).
        /// </summary>
        public Transform() {

            position = new Vector2();
        }

        // <summary>
        /// Constructs a new Transform by copying another Transform's properties.
        /// </summary>
        /// <param name="transform">The Transform to copy from.</param>
        public Transform(Transform transform) {

            position = transform.position;
            size = transform.size;
            rotation = transform.rotation;
            mobility = transform.mobility;
            parent = transform.parent;

        }

        /// <summary>
        /// Constructs a new Transform with specified parameters.
        /// </summary>
        /// <param name="position">The position of the Transform.</param>
        /// <param name="size">The size of the Transform.</param>
        /// <param name="rotation">The rotation of the Transform in degrees.</param>
        /// <param name="mobility">The mobility type of the Transform.</param>
        /// <param name="parent">The parent Transform, if any.</param>
        public Transform(Vector2? position = null, Vector2? size = null, float rotation = 0, Mobility mobility = Mobility.DYNAMIC, Transform? parent = null) {

            _position = position ?? new Vector2();
            this.size = size ?? new Vector2();
            this.rotation = rotation;
            this.mobility = mobility;
            this.parent = parent;
        }

        /// <summary>
        /// Gets or sets the position of the Transform, considering the parent's position if it exists.
        /// </summary>
        public Vector2 position {
            get {
                if(parent == null)
                    return _position;
                else 
                    return parent.position + _position;
            }

            set { _position = parent == null ? value : value - parent.position; }
        }

        /// <summary>
        /// Adds two Transforms together, resulting in a new Transform with combined properties.
        /// </summary>
        public static Transform operator +(Transform t1, Transform t2) {

            return new Transform {
                _position = t1.position + t2.position,
                size = t1.size + t2.size,
                rotation = t1.rotation + t2.rotation,
                mobility = (Mobility)Math.Max((int)t2.mobility, (int)t1.mobility),
                parent = null // When adding two transforms, the result should not have a parent
            };
        }

        /// <summary>
        /// Retrieves the transformation matrix representing the Transform's position, size, and rotation.
        /// </summary>
        /// <returns>The transformation matrix of the Transform.</returns>
        public Matrix4 GetTransformationMatrix() {

            Vector2 position = this.position;
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(size.X, size.Y, 1));
            Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.rotation));

            return scale * rotation * translation;
        }

        /// <summary>
        /// Transforms a point using the Transform's transformation matrix.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <returns>The transformed point.</returns>
        public Vector2 TransformPoint(Vector2 point) {

            Vector4 homogenousPoint = new Vector4(point.X, point.Y, 0, 1);
            Vector4 transformedPoint = GetTransformationMatrix() * homogenousPoint;
            return new Vector2(transformedPoint.X, transformedPoint.Y);
        }

        /// <summary>
        /// Inversely transforms a point using the inverse of the Transform's transformation matrix.
        /// </summary>
        /// <param name="point">The point to inversely transform.</param>
        /// <returns>The inversely transformed point.</returns>
        public Vector2 InverseTransformPoint(Vector2 point) {

            Matrix4 inverseTransform = Matrix4.Invert(GetTransformationMatrix());
            Vector4 homogenousPoint = new Vector4(point.X, point.Y, 0, 1);
            Vector4 transformedPoint = inverseTransform * homogenousPoint;
            return new Vector2(transformedPoint.X, transformedPoint.Y);
        }

        /// <summary>
        /// Moves the Transform in a specified direction with a given speed.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        /// <param name="speed">The speed of movement.</param>
        public void Move(Vector2 direction, float speed) {

            direction = RotateVector(direction, rotation);
            Vector2 velocity = direction * speed * Game_Time.delta;
            position += velocity;
        }

        // ------------------------------------ private ------------------------------------

        private Vector2 _position;

        private Vector2 RotateVector(Vector2 vector, float degrees) {

            float radians = MathHelper.DegreesToRadians(degrees);
            float x = vector.X * MathF.Cos(radians) - vector.Y * MathF.Sin(radians);
            float y = vector.X * MathF.Sin(radians) + vector.Y * MathF.Cos(radians);
            return new Vector2(x, y);
        }
    }
}