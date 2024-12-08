using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core.Particles {
    public class Emitter {
        public Vector2 Position;
        public float EmissionRate;
        public bool Continuous;
        public float ParticleLifetime;
        public Func<Vector2> VelocityFunction;
        public Func<float> SizeFunction;
        public Func<float> RotationFunction;
        public ColorGradient ColorGradient;
        public bool IsActive { get; protected set; } = true;
        public Func<bool> IsAffectedByForcesFunction;

        protected float _emissionAccumulator = 0f;
        protected int _particlesEmitted = 0;
        protected int _maxParticles;

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
            int maxParticles = int.MaxValue) {
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
        }

        virtual public void Emit(List<Particle> particles, float deltaTime) {

            if(!IsActive) return;

            _emissionAccumulator += EmissionRate * deltaTime;
            int numNewParticles = (int)_emissionAccumulator;
            _emissionAccumulator -= numNewParticles;

            for(int i = 0; i < numNewParticles; i++) {
                if(particles.Count < ParticleSystem.MaxParticles && _particlesEmitted < _maxParticles) {
                    var particle = new Particle(
                        Position,
                        VelocityFunction(),
                        SizeFunction(),
                        RotationFunction(),
                        ParticleLifetime,
                        ColorGradient,
                        IsAffectedByForcesFunction()
                    );

                    particles.Add(particle);
                    _particlesEmitted++;

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
