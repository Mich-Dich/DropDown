using OpenTK.Mathematics;

namespace Core.Particles {
    public class XPParticle : Particle {
        public enum XPParticleState {
            Waiting,
            Attracted,
            Collected
        }

        public XPParticleState State { get; private set; } = XPParticleState.Waiting;

        // Configurable parameters
        public float AttractDistance { get; set; }
        public float CollectDistance { get; set; }
        public float MaxAttractForce { get; set; }
        public float MaxSpeed { get; set; }
        public float Damping { get; set; } = 0.95f; // Damping factor for smoother motion

        public XPParticle(Vector2 position, float size, ColorGradient colorGradient, float attractDistance, float collectDistance, float maxAttractForce, float maxSpeed)
            : base(position, Vector2.Zero, size, 0f, float.MaxValue, colorGradient, isAffectedByForces: false) {

            AttractDistance = attractDistance;
            CollectDistance = collectDistance;
            MaxAttractForce = maxAttractForce;
            MaxSpeed = maxSpeed;
        }

        public void Update(Vector2 playerPosition, float deltaTime) {
            Vector2 toPlayer = playerPosition - Position;
            float distance = toPlayer.Length;

            switch(State) {
            case XPParticleState.Waiting:
            if(distance <= AttractDistance) {
                State = XPParticleState.Attracted;
            }
            break;

            case XPParticleState.Attracted:
            if(distance > CollectDistance) {
                Vector2 direction = toPlayer / distance;

                // Linear attenuation based on distance
                float forceMagnitude = MaxAttractForce * (1.0f - (distance / AttractDistance));
                forceMagnitude = MathHelper.Clamp(forceMagnitude, 0.0f, MaxAttractForce);

                // Apply acceleration towards the player
                Vector2 acceleration = direction * forceMagnitude;
                Velocity += acceleration * deltaTime;

                // Apply damping to smooth out the motion
                Velocity *= MathF.Pow(Damping, deltaTime);

                // Clamp velocity to MaxSpeed
                if(Velocity.Length > MaxSpeed) {
                    Velocity = Velocity.Normalized() * MaxSpeed;
                }
            }
            else {
                State = XPParticleState.Collected;
            }
            break;

            case XPParticleState.Collected:
            Age = LifeTime; // Mark as dead
            break;
            }

            base.Update(deltaTime);
        }
    }
}
