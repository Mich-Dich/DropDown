using OpenTK.Mathematics;

namespace Core.particle
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public ColorGradient ColorGradient;
        public float Size;
        public float Life;
        public float Rotation;
        public bool IsShockwave;

        public Particle(Vector2 position, Vector2 velocity, ColorGradient colorGradient, float size, float life, float rotation, bool isShockwave = false)
        {
            Position = position;
            Velocity = velocity;
            ColorGradient = colorGradient;
            Size = size;
            Life = life;
            Rotation = rotation;
            IsShockwave = isShockwave;
        }

        public Vector4 GetCurrentColor()
        {
            float t = 1.0f - Life;
            return ColorGradient.GetColor(t);
        }
    }
}