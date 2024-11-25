using OpenTK.Mathematics;

namespace Core.Particles
{
    public class AttractionForceField : IForceField
    {
        public Vector2 Position { get; set; }
        public float Strength { get; set; }

        public AttractionForceField(Vector2 position, float strength)
        {
            Position = position;
            Strength = strength;
        }

        public void ApplyForce(Particle particle, float deltaTime)
        {
            if (particle.IsAffectedByForces)
            {
                Vector2 direction = Position - particle.Position;
                float distance = direction.Length;
                if (distance > 0)
                {
                    direction /= distance;
                    Vector2 force = direction * Strength * deltaTime;
                    particle.Velocity += force;
                }
            }
        }
    }
}
