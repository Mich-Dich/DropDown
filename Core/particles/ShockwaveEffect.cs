using Core.world;
using OpenTK.Mathematics;

namespace Core.Particles {
    public static class ShockwaveEffect {
        public static void Create(Map map, Vector2 position, int particleCount, float maxSpeed, float lifetime, float scale) {

            Console.WriteLine($"Creating shockwave at position: {position}, particle count: {particleCount}");

            float adjustedScale = scale * 0.3f;

            // Color gradient
            ColorGradient colorGradient = new ColorGradient();
            colorGradient.AddColor(0.0f, new Vector4(0.0f, 0.8f, 1.0f, 1.0f)); // Bright cyan
            colorGradient.AddColor(0.3f, new Vector4(0.0f, 0.6f, 1.0f, 0.7f)); // Medium blue
            colorGradient.AddColor(0.6f, new Vector4(0.0f, 0.4f, 1.0f, 0.4f)); // Darker blue
            colorGradient.AddColor(1.0f, new Vector4(0.0f, 0.0f, 0.5f, 0.0f)); // Dark blue, fade out

            Random random = new Random();
            List<Particle> particles = new List<Particle>();

            float maxRadius = 0.5f * adjustedScale;

            for(int i = 0; i < particleCount; i++) {
                float radiusRandom = (float)Math.Pow(random.NextDouble(), 0.5);
                float startRadius = maxRadius * radiusRandom;

                float theta = (float)(random.NextDouble() * MathHelper.TwoPi);
                float phi = (float)(random.NextDouble() * MathHelper.Pi);

                Vector2 startPosition = position + new Vector2(
                    startRadius * (float)Math.Sin(phi) * (float)Math.Cos(theta),
                    startRadius * (float)Math.Sin(phi) * (float)Math.Sin(theta)
                );

                Vector2 direction = (startPosition - position).Normalized();
                float angleVariation = (float)(random.NextDouble() * 0.6f - 0.3f);
                direction = new Vector2(
                    direction.X * MathF.Cos(angleVariation) - direction.Y * MathF.Sin(angleVariation),
                    direction.X * MathF.Sin(angleVariation) + direction.Y * MathF.Cos(angleVariation)
                );

                float speedVariation = (float)(random.NextDouble() * 0.5f + 0.75f);
                float particleSpeed = maxSpeed * speedVariation * (1.0f - radiusRandom);

                Vector2 velocity = direction * particleSpeed * adjustedScale;

                float lifeTimeVariation = (float)(random.NextDouble() * 0.2f - 0.1f);
                float particleLifetime = lifetime + lifeTimeVariation;

                float sizeVariation = (float)(random.NextDouble() * 0.5f + 0.75f);
                float size = 0.02f * sizeVariation * adjustedScale;

                Func<float, float> sizeOverLifeFunction = t => MathF.Pow(MathF.Sin(MathF.PI * t), 0.5f);

                var particle = new Particle(
                    startPosition,
                    velocity,
                    size,
                    rotation: 0f,
                    lifeTime: particleLifetime,
                    colorGradient: colorGradient,
                    isAffectedByForces: false,
                    sizeOverLifeFunction: sizeOverLifeFunction
                );

                particles.Add(particle);
            }

            map.AddParticles(particles);
        }
    }
}
