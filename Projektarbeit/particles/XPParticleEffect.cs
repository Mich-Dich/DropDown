using System;
using System.Collections.Generic;
using Core.Particles;
using OpenTK.Mathematics;

namespace Projektarbeit.particles
{
    public static class XPParticleEffect
    {
        public static void Create(
            ParticleSystem particleSystem, 
            int amount,
            Vector2 position,
            float attractDistance = 5.0f,
            float collectDistance = 0.5f,
            float maxAttractForce = 30.0f,
            float maxSpeed = 5.0f,
            float damping = 0.95f
        )
        {
            Random random = new Random();
            List<Particle> particles = new List<Particle>();

            // We'll define a color range from (0.0, 0.0, 1.0) to (0.7, 0.2, 1.0) for a bluish/purple look
            Vector3 colorStart = new Vector3(0.0f, 0.0f, 1.0f); // Blue
            Vector3 colorEnd   = new Vector3(0.7f, 0.2f, 1.0f); // More pink/purple

            for (int i = 0; i < amount; i++)
            {
                // Random offset so they don't all spawn exactly at 'position'
                float offsetX = (float)(random.NextDouble() * 0.4f - 0.2f);
                float offsetY = (float)(random.NextDouble() * 0.4f - 0.2f);
                Vector2 spawnPos = position + new Vector2(offsetX, offsetY);

                // Random size, e.g. 3 to 7
                float size = (float)(random.NextDouble() * 4f + 3f);

                // Lerp color from colorStart to colorEnd
                float t = (float)random.NextDouble();
                Vector3 colorVec = Vector3.Lerp(colorStart, colorEnd, t);
                Vector4 finalColor = new Vector4(colorVec.X, colorVec.Y, colorVec.Z, 1.0f);

                // Create a color gradient that doesn't change over lifetime
                ColorGradient xpColorGradient = new ColorGradient();
                xpColorGradient.AddColor(0.0f, finalColor);
                xpColorGradient.AddColor(1.0f, finalColor);

                // Construct the XPParticle with the desired attraction properties
                var xpParticle = new XPParticle(
                    position: spawnPos,
                    size: size,
                    colorGradient: xpColorGradient,
                    attractDistance: attractDistance,
                    collectDistance: collectDistance,
                    maxAttractForce: maxAttractForce,
                    maxSpeed: maxSpeed,
                    damping: damping
                );

                particles.Add(xpParticle);
            }

            particleSystem.AddParticles(particles);
        }
    }
}
