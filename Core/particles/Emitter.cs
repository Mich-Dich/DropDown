using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core.Particles
{
    public class Emitter
    {
        public Vector2 Position;
        public float EmissionRate;
        public bool Continuous;
        public float ParticleLifetime;
        public Func<Vector2> VelocityFunction;
        public Func<float> SizeFunction;
        public Func<float> RotationFunction;
        public ColorGradient ColorGradient;
        public bool IsActive { get; private set; } = true;
        public Func<bool> IsAffectedByForcesFunction;
        public Func<float, float> SizeOverLifeFunction;

        private float _emissionAccumulator = 0f;
        private int _particlesEmitted = 0;
        private int _maxParticles;
        private Random _random = new Random();

        // Increase maxRadius for bigger initial ring, add randomness
        private float maxRadius = 0.6f; // slightly bigger than 0.5f
        // We can also add some noise scale
        private float positionNoise = 0.2f; // Add random noise to position

        public Emitter(
            Vector2 position,
            float emissionRate,
            bool continuous,
            float particleLifetime,
            Func<Vector2> velocityFunction,
            Func<float> sizeFunction,
            Func<float> rotationFunction,
            ColorGradient colorGradient,
            Func<bool> isAffectedByForcesFunction = null,
            int maxParticles = int.MaxValue,
            Func<float, float> sizeOverLifeFunction = null
        ) {
            Position = position;
            EmissionRate = emissionRate;
            Continuous = continuous;
            ParticleLifetime = particleLifetime;
            VelocityFunction = velocityFunction;
            SizeFunction = sizeFunction;
            RotationFunction = rotationFunction;
            ColorGradient = colorGradient;
            IsAffectedByForcesFunction = isAffectedByForcesFunction ?? (() => true);
            _maxParticles = maxParticles;
            SizeOverLifeFunction = sizeOverLifeFunction ?? (t => 1.0f);
        }

        public void Emit(List<Particle> particles, float deltaTime)
        {
            if (!IsActive) return;

            _emissionAccumulator += EmissionRate * deltaTime;
            int numNewParticles = (int)_emissionAccumulator;
            _emissionAccumulator -= numNewParticles;

            for (int i = 0; i < numNewParticles; i++)
            {
                if (particles.Count < ParticleSystem.MaxParticles && _particlesEmitted < _maxParticles)
                {
                    // Random radius and angle for initial ring distribution
                    float radiusRandom = (float)Math.Sqrt(_random.NextDouble()); // sqrt distribution
                    float startRadius = maxRadius * radiusRandom;
                    float angle = (float)(_random.NextDouble() * MathHelper.TwoPi);

                    Vector2 offset = new Vector2(
                        MathF.Cos(angle) * startRadius,
                        MathF.Sin(angle) * startRadius
                    );

                    // Add noise to the position to break up the perfect ring
                    offset.X += (float)(_random.NextDouble() * 2 - 1) * positionNoise;
                    offset.Y += (float)(_random.NextDouble() * 2 - 1) * positionNoise;

                    Vector2 startPos = Position + offset;

                    // Get a base velocity
                    Vector2 velocity = VelocityFunction();

                    // Add more angle and speed noise to velocity to break linearity
                    float extraAngle = (float)(_random.NextDouble() * 0.8f - 0.4f); // +/- 0.4 radians extra variation
                    float cosA = MathF.Cos(extraAngle);
                    float sinA = MathF.Sin(extraAngle);
                    velocity = new Vector2(
                        velocity.X * cosA - velocity.Y * sinA,
                        velocity.X * sinA + velocity.Y * cosA
                    );

                    // Add random speed noise:
                    float speedNoise = (float)(_random.NextDouble() * 0.5f + 0.75f); // another layer of variation
                    velocity *= speedNoise;

                    // Random lifetime variation
                    float lifetimeVariation = (float)(_random.NextDouble() * 0.2f - 0.1f); // +/- 0.1s variation
                    float finalLifetime = ParticleLifetime + lifetimeVariation;

                    // Random size variation
                    float baseSize = SizeFunction();
                    float sizeVar = (float)(_random.NextDouble() * 0.5f + 0.75f); // vary size by 0.75x to 1.25x
                    float finalSize = baseSize * sizeVar;

                    // Random rotation variation
                    float baseRotation = RotationFunction();
                    float rotationNoise = (float)(_random.NextDouble() * MathHelper.TwoPi); // random rotation
                    float finalRotation = baseRotation + rotationNoise;

                    // Create particle
                    var particle = new Particle(
                        startPos,
                        velocity,
                        finalSize,
                        finalRotation,
                        finalLifetime,
                        ColorGradient,
                        isAffectedByForces: IsAffectedByForcesFunction(),
                        sizeOverLifeFunction: SizeOverLifeFunction
                    );

                    particles.Add(particle);
                    _particlesEmitted++;

                    if (!Continuous && _particlesEmitted >= _maxParticles)
                    {
                        IsActive = false;
                        break;
                    }
                }
                else
                {
                    IsActive = false;
                    break;
                }
            }
        }
    }
}
