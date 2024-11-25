namespace Core.Particles
{
    public interface IForceField
    {
        void ApplyForce(Particle particle, float deltaTime);
    }
}
