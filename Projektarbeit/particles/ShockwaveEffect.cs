using System;
using Core.Particles;
using OpenTK.Mathematics;

namespace Projektarbeit.particles
{
    public static class ShockwaveEffect
    {
        public static void Trigger(
            ParticleSystem particleSystem,
            Vector2 position,
            float scale = 10.0f,
            float maxSpeed = 50.0f,
            float particleLifetime = 0.4f,
            int maxParticles = 2000
        )
        {
            // Setup the color gradient (same as in MAP_base)
            ColorGradient colorGradient = new ColorGradient();
            colorGradient.AddColor(0.0f, new Vector4(0.0f, 0.8f, 1.0f, 1.0f));
            colorGradient.AddColor(0.3f, new Vector4(0.0f, 0.6f, 1.0f, 0.7f));
            colorGradient.AddColor(0.6f, new Vector4(0.0f, 0.4f, 1.0f, 0.4f));
            colorGradient.AddColor(1.0f, new Vector4(0.0f, 0.0f, 0.5f, 0.0f));

            // The bubble size function
            Func<float, float> sizeOverLifeFunction = t => MathF.Pow(MathF.Sin(MathF.PI * t), 0.5f);

            // Define velocity function
            var random = new Random();
            Func<Vector2> velocityFunction = () =>
            {
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

                float speedVariation = (float)(random.NextDouble() * 0.5 + 0.75); // 0.75 - 1.25
                float particleSpeed = maxSpeed * speedVariation;
                float adjustedScale = scale * 0.3f;
                return direction * particleSpeed * adjustedScale;
            };

            // Simple size function
            Func<float> sizeFunction = () => 8.0f;

            // Rotation always zero in your example
            Func<float> rotationFunction = () => 0f;

            // No forces
            Func<bool> isAffectedByForcesFunction = () => false;

            // Add the emitter as a single burst
            particleSystem.AddEmitter(new Emitter(
                position: position,
                emissionRate: 1000000,
                continuous: false,
                particleLifetime: particleLifetime,
                velocityFunction,
                sizeFunction,
                rotationFunction,
                colorGradient,
                isAffectedByForcesFunction,
                maxParticles: maxParticles,
                sizeOverLifeFunction: sizeOverLifeFunction
            ));
        }
    }
}
