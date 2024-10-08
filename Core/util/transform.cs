using Core.world;
using OpenTK.Mathematics;
using System;

namespace Core.util
{
    public class Transform
    {
        public Vector2 size { get; set; } = new Vector2(0);
        public float rotation { get; set; } = 0;
        public Mobility mobility { get; set; } = Mobility.DYNAMIC;
        public Transform? parent { get; set; }

        private float shakeIntensity = 0;
        private float shakeDecay = 0.98f;
        private Vector2 originalPosition;
        private bool isShaking = false;

        private static Random random = new Random();

        public Transform() { this.position = default(Vector2); }

        private float shakeTimer = 0f;
        private ShakeProfile currentShakeProfile = null;


        public Transform(Transform transform)
        {
            this.position = transform.position;
            this.size = transform.size;
            this.rotation = transform.rotation;
            this.mobility = transform.mobility;
            this.parent = transform.parent;
        }

        public Transform(Vector2? position = null, Vector2? size = null, float rotation = 0, Mobility mobility = Mobility.DYNAMIC, Transform? parent = null)
        {
            this.position = position ?? default(Vector2);
            this.size = size ?? default(Vector2);
            this.rotation = rotation;
            this.mobility = mobility;
            this.parent = parent;
        }

        private Vector2 positionValue;

        public Vector2 position
        {
            get
            {
                if (this.parent == null)
                    return this.positionValue;
                else
                    return this.parent.position + this.positionValue;
            }

            set { this.positionValue = this.parent == null ? value : value - this.parent.position; }
        }

        public static Transform operator +(Transform t1, Transform t2)
        {
            return new Transform
            {
                position = t1.position + t2.position,
                size = t1.size + t2.size,
                rotation = t1.rotation + t2.rotation,
                mobility = (Mobility)Math.Max((int)t2.mobility, (int)t1.mobility),
                parent = null,
            };
        }

        public Matrix4 GetTransformationMatrix()
        {
            Vector2 position = this.position;
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(this.size.X, this.size.Y, 1));
            Matrix4 rotation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.rotation));

            return scale * rotation * translation;
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            Vector4 homogenousPoint = new(point.X, point.Y, 0, 1);
            Vector4 transformedPoint = this.GetTransformationMatrix() * homogenousPoint;
            return new Vector2(transformedPoint.X, transformedPoint.Y);
        }

        public Vector2 InverseTransformPoint(Vector2 point)
        {
            Matrix4 inverseTransform = Matrix4.Invert(this.GetTransformationMatrix());
            Vector4 homogenousPoint = new(point.X, point.Y, 0, 1);
            Vector4 transformedPoint = inverseTransform * homogenousPoint;
            return new Vector2(transformedPoint.X, transformedPoint.Y);
        }

        public void Move(Vector2 direction, float speed)
        {
            direction = this.RotateVector(direction, this.rotation);
            Vector2 velocity = direction * speed * Game_Time.delta;
            this.position += velocity;
        }

        public void ApplyShake(ShakeProfile profile)
        {
            Console.WriteLine($"Applying shake with intensity: {profile.Intensity}, decay: {profile.Decay}, frequency: {profile.Frequency}");
            if (currentShakeProfile == null || shakeIntensity < profile.Intensity)
            {
                currentShakeProfile = profile;
                shakeIntensity = profile.Intensity;
                shakeDecay = profile.Decay;
                if (!isShaking)
                {
                    originalPosition = this.position;
                    isShaking = true;
                    Console.WriteLine("Shaking started.");
                }
                shakeTimer = 0f;
            }
        }

        public void Update()
        {
            if (isShaking)
            {
                shakeTimer += Game_Time.delta;
                if (shakeTimer >= (1f / currentShakeProfile.Frequency))
                {
                    shakeTimer = 0f;
                    float shakeMagnitude = 10f;
                    Vector2 shakeAmount = new Vector2(
                        (float)(random.NextDouble() * 2 - 1) * shakeIntensity * shakeMagnitude,
                        (float)(random.NextDouble() * 2 - 1) * shakeIntensity * shakeMagnitude);
                    Console.WriteLine($"Applying shake amount: {shakeAmount}");
                    position += shakeAmount;

                    shakeIntensity *= shakeDecay;
                    position = Vector2.Lerp(position, originalPosition, Math.Clamp(1 - shakeIntensity, 0, 1));

                    if (shakeIntensity < 0.01f)
                    {
                        shakeIntensity = 0;
                        position = originalPosition;
                        isShaking = false;
                        currentShakeProfile = null;
                        Console.WriteLine("Shaking stopped.");
                    }
                }
            }
        }

        public override string ToString()
        {
            if (this.parent != null)
                return $"position: {this.position} size: {this.size} rotation: [{this.rotation}] mobility: [{this.mobility}]\n" +
                       $"      parent: [{this.parent}]";
            else
                return $"position: {this.position} size: {this.size} rotation: [{this.rotation}] mobility: [{this.mobility}]";
        }

        private Vector2 RotateVector(Vector2 vector, float degrees)
        {
            float radians = MathHelper.DegreesToRadians(degrees);
            float x = (vector.X * MathF.Cos(radians)) - (vector.Y * MathF.Sin(radians));
            float y = (vector.X * MathF.Sin(radians)) + (vector.Y * MathF.Cos(radians));
            return new Vector2(x, y);
        }
    }
}