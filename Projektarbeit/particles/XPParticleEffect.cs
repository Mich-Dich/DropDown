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
            var particles = new List<Particle>();

            // Example: We'll define a color range from (0.0,0.0,1.0) (blue) to (0.7,0.2,1.0) (pink/purple)
            Vector3 colorStart = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 colorEnd =   new Vector3(0.7f, 0.2f, 1.0f);

            // A bigger base radius so they're clearly spaced.
            // With 10 particles, a radius ~8 is visibly spread; with 100 particles, they fill a large circle.
            float scatterRadius = 8.0f;  

            // For variety in size:
            // We’ll pick a random base size in [3..7], then multiply by ~[0.8..1.2].
            float minBaseSize = 3.0f;
            float maxBaseSize = 7.0f;

            for (int i = 0; i < amount; i++)
            {
                // 1) Pick a random radius [0..scatterRadius] (NO sqrt distribution here, so more uniform).
                float r = (float)(random.NextDouble() * scatterRadius);

                // 2) Random angle in [0..2π].
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);

                // 3) Convert polar -> cartesian.
                float offsetX = r * MathF.Cos(angle);
                float offsetY = r * MathF.Sin(angle);

                // Final spawn position
                Vector2 spawnPos = position + new Vector2(offsetX, offsetY);

                // 4) Choose a random base size in [3..7].
                float baseSize = (float)(minBaseSize + random.NextDouble() * (maxBaseSize - minBaseSize));

                // 5) Extra ±20% variation factor => final size
                float sizeVariationFactor = 0.8f + (float)(random.NextDouble() * 0.4f);
                float finalSize = baseSize * sizeVariationFactor;

                // 6) Random color
                float t = (float)random.NextDouble();
                Vector3 colorVec = Vector3.Lerp(colorStart, colorEnd, t);
                Vector4 finalColor = new Vector4(colorVec.X, colorVec.Y, colorVec.Z, 1.0f);

                // Single-color gradient => same color at 0.0 & 1.0
                var xpColorGradient = new ColorGradient();
                xpColorGradient.AddColor(0.0f, finalColor);
                xpColorGradient.AddColor(1.0f, finalColor);

                // Create the XPParticle
                var xpParticle = new XPParticle(
                    position:        spawnPos,
                    size:            finalSize,
                    colorGradient:   xpColorGradient,
                    attractDistance: attractDistance,
                    collectDistance: collectDistance,
                    maxAttractForce: maxAttractForce,
                    maxSpeed:        maxSpeed,
                    damping:         damping
                );

                particles.Add(xpParticle);
            }

            // Finally add them to the system
            particleSystem.AddParticles(particles);
        }
    }
}
