using OpenTK.Mathematics;

namespace Core.Particles
{
    public class GravityForceField : IForceField
    {
        public Vector2 Gravity { get; set; }

        public GravityForceField(Vector2 gravity)
        {
            Gravity = gravity;
        }

        public void ApplyForce(Particle particle, float deltaTime)
        {
            if (particle.IsAffectedByForces)
            {
                particle.Velocity += Gravity * deltaTime;
            }
        }
    }
}
