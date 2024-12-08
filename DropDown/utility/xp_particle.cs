using Core.Particles;
using OpenTK.Mathematics;

namespace DropDown.utility {
    public class xp_particle : Particle {

        private const float ATTRACTION_RANGE = 200f;
        private const float ATTRACTION_SPEED = 300f;
        private bool isAttracting = false;

        public xp_particle(Vector2 position, Vector2 velocity, float size, float rotation, float lifeTime, ColorGradient colorGradient)
            : base(position, velocity, size, rotation, lifeTime, colorGradient) { }

        public void Update(Vector2 playerPosition, float deltaTime) {

            Console.WriteLine($"updating xp_particle");

            float distanceToPlayer = Vector2.Distance(Position, playerPosition);
            Console.WriteLine($"playerPosition: {playerPosition}  Position: {Position}     DISTANCE: {distanceToPlayer}");

            if(distanceToPlayer < ATTRACTION_RANGE) 
                isAttracting = true;

            if(isAttracting) {
                Vector2 directionToPlayer = (playerPosition - Position).Normalized();
                Velocity = directionToPlayer * ATTRACTION_SPEED;
            }

            if(distanceToPlayer < 20f) { // Collection distance

                Age = LifeTime;
                // trigger XP collection logic
            }

            base.Update(deltaTime);
        }
    }

    public class XP_emitter : Emitter {

        private const float XP_PARTICLE_LIFETIME = 10f;
        private const float XP_PARTICLE_SIZE = 15f;
        private int particle_count_max = 0;
        private int particle_count = 0;

        public XP_emitter(Vector2 position, int num_of_particles, bool continuous = true, int maxParticles = int.MaxValue)
            : base(
                position,
                900,
                continuous,
                XP_PARTICLE_LIFETIME,
                () => {
                    float angle = Random.Shared.NextSingle() * MathHelper.TwoPi;
                    return Random.Shared.NextSingle() * new Vector2(
                        MathF.Cos(angle) * 15f,
                        MathF.Sin(angle) * 15f
                    );
                },
                () => XP_PARTICLE_SIZE,
                () => 0f,
                new ColorGradient(new[] {
                    (0.0f, new Vector4(0.0f, 1f, 0.0f, 1.0f)),  // Green
                    (1.0f, new Vector4(0.0f, 0.8f, 0.0f, 0.0f)) // Fade out
                }),
                () => true,
                maxParticles) {

            particle_count_max = num_of_particles;
        }

        public override void Emit(List<Particle> particles, float deltaTime) {

            if(!IsActive)
                return;

            if (particle_count < particle_count_max) {

                _emissionAccumulator += EmissionRate * deltaTime;
                int numNewParticles = (int)_emissionAccumulator;
                _emissionAccumulator -= numNewParticles;

                for(int i = particle_count; i <= particle_count_max; i++) {

                    if(particles.Count < ParticleSystem.MaxParticles && _particlesEmitted < _maxParticles) {

                        float angle = Random.Shared.NextSingle() * MathHelper.TwoPi;
                        Vector2 position_offset = Random.Shared.NextSingle() * new Vector2(
                            MathF.Cos(angle) * 50f,
                            MathF.Sin(angle) * 50f
                        );

                        var particle = new xp_particle(
                            Position + position_offset,
                            VelocityFunction(),
                            SizeFunction(),
                            RotationFunction(),
                            ParticleLifetime,
                            ColorGradient
                        );

                        particles.Add(particle);
                        _particlesEmitted++;
                        particle_count++;

                        if(!Continuous && _particlesEmitted >= _maxParticles) {
                            IsActive = false;
                            break;
                        }
                    }
                    else {
                        IsActive = false;
                        break;
                    }
                }

            }
            
        }
    }
}
