using OpenTK.Mathematics;
using Core.util;

namespace Core.particle
{
    public class Emitter
    {
        public Transform Transform { get; set; }
        public Vector2 Velocity;
        public float EmissionRate;
        public bool Continuous;
        public ColorGradient ColorGradient;
        public float MinSize;
        public float MaxSize;
        public float MinRotation;
        public float MaxRotation;

        public Emitter(Transform transform, Vector2 velocity, float emissionRate, bool continuous, ColorGradient colorGradient, float minSize, float maxSize, float minRotation, float maxRotation)
        {
            Transform = transform;
            Velocity = velocity;
            EmissionRate = emissionRate;
            Continuous = continuous;
            ColorGradient = colorGradient;
            MinSize = minSize;
            MaxSize = maxSize;
            MinRotation = minRotation;
            MaxRotation = maxRotation;
        }
    }
}