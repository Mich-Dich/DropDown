using OpenTK.Mathematics;

namespace Core.Particles
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Size;
        public float Rotation;
        public float LifeTime;
        public float Age;
        public ColorGradient ColorGradient;
        public bool IsAffectedByForces;
        public Func<float, float> SizeOverLifeFunction;

        public Particle(Vector2 position, Vector2 velocity, float size, float rotation, float lifeTime, ColorGradient colorGradient, bool isAffectedByForces = true, Func<float, float> sizeOverLifeFunction = null)
        {
            Position = position;
            Velocity = velocity;
            Size = size;
            Rotation = rotation;
            LifeTime = lifeTime;
            Age = 0f;
            ColorGradient = colorGradient;
            IsAffectedByForces = isAffectedByForces;
            SizeOverLifeFunction = sizeOverLifeFunction ?? (t => 1.0f);
        }

        public void Update(float deltaTime)
        {
            Age += deltaTime;
            Position += Velocity * deltaTime;
        }

        public bool IsAlive => Age < LifeTime;

        public Vector4 GetCurrentColor()
        {
            float t = Age / LifeTime;
            return ColorGradient.GetColor(t);
        }

        public float GetCurrentSize()
        {
            float t = Age / LifeTime;
            return Size * SizeOverLifeFunction(t);
        }
    }
}
