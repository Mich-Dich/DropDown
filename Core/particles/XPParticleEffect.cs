using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core.Particles
{
    public static class XPParticleEffect
    {
        public static void Create(ParticleSystem particleSystem, int amount, Vector2 position)
        {
            Random random = new Random();
            List<Particle> particles = new List<Particle>();

            // Define color spectrum (blue to purple)
            Vector3 colorStart = new Vector3(0.0f, 0.0f, 1.0f); // Blue
            Vector3 colorEnd = new Vector3(0.5f, 0.0f, 1.0f);   // Purple

            // Configurable parameters
            float attractDistance = 0.2f;
            float collectDistance = 0.02f;
            float maxAttractForce = 10.0f;
            float maxSpeed = 1.0f;

            for (int i = 0; i < amount; i++)
            {
                // Random position offset
                float offsetX = (float)(random.NextDouble() * 0.05f - 0.025f);
                float offsetY = (float)(random.NextDouble() * 0.05f - 0.025f);
                Vector2 particlePosition = position + new Vector2(offsetX, offsetY);

                // Random size
                float size = (float)(random.NextDouble() * 0.005f + 0.01f); // Sizes between 0.01 and 0.015

                // Random color within the spectrum
                float t = (float)random.NextDouble();
                Vector3 colorVec3 = Vector3.Lerp(colorStart, colorEnd, t);
                Vector4 color = new Vector4(colorVec3.X, colorVec3.Y, colorVec3.Z, 1.0f);

                // Create a color gradient for the particle (constant color)
                ColorGradient particleColorGradient = new ColorGradient();
                particleColorGradient.AddColor(0.0f, color);
                particleColorGradient.AddColor(1.0f, color);

                // Create XPParticle
                var particle = new XPParticle(
                    particlePosition,
                    size,
                    particleColorGradient,
                    attractDistance,
                    collectDistance,
                    maxAttractForce,
                    maxSpeed
                );

                particles.Add(particle);
            }

            particleSystem.AddParticles(particles);
        }
    }
}
