using System;
using System.Collections.Generic;
using Core.Particles;
using OpenTK.Mathematics;

namespace Projektarbeit.particles
{
    public enum XPOrbType {
        Small,
        Medium,
        Large,
        Super
    }

    public static class XPParticleEffect
    {
        // New: Create XP orbs by type
        public static void CreateByType(
            ParticleSystem particleSystem,
            int amount,
            Vector2 position,
            XPOrbType orbType,
            float attractDistance = 5.0f,
            float collectDistance = 0.3f,
            float maxAttractForce = 40.0f,
            float maxSpeed = 6.0f,
            float damping = 0.92f
        )
        {
            Random random = new Random();
            var particles = new List<Particle>();

            // Color and size by type
            Vector3 colorStart, colorEnd;
            float minBaseSize, maxBaseSize;
            switch (orbType)
            {
                case XPOrbType.Small:
                    colorStart = new Vector3(0.2f, 0.6f, 1.0f); // blue
                    colorEnd = new Vector3(0.5f, 0.3f, 1.0f);   // purple
                    minBaseSize = 3.0f; maxBaseSize = 5.0f;
                    break;
                case XPOrbType.Medium:
                    colorStart = new Vector3(0.5f, 0.3f, 1.0f); // purple
                    colorEnd = new Vector3(0.8f, 0.4f, 1.0f);   // magenta
                    minBaseSize = 5.0f; maxBaseSize = 8.0f;
                    break;
                case XPOrbType.Large:
                    colorStart = new Vector3(0.8f, 0.4f, 1.0f); // magenta
                    colorEnd = new Vector3(1.0f, 0.8f, 0.2f);   // gold
                    minBaseSize = 8.0f; maxBaseSize = 12.0f;
                    break;
                case XPOrbType.Super:
                default:
                    colorStart = new Vector3(1.0f, 0.8f, 0.2f); // gold
                    colorEnd = new Vector3(0.7f, 1.0f, 1.0f);   // cyan
                    minBaseSize = 13.0f; maxBaseSize = 18.0f;
                    break;
            }

            float scatterRadius = 8.0f;  

            for (int i = 0; i < amount; i++)
            {
                float r = (float)(random.NextDouble() * scatterRadius);
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                float offsetX = r * MathF.Cos(angle);
                float offsetY = r * MathF.Sin(angle);
                Vector2 spawnPos = position + new Vector2(offsetX, offsetY);

                float baseSize = (float)(minBaseSize + random.NextDouble() * (maxBaseSize - minBaseSize));
                float sizeVariationFactor = 0.8f + (float)(random.NextDouble() * 0.4f);
                float finalSize = baseSize * sizeVariationFactor;

                float t = (float)random.NextDouble();
                Vector3 colorVec = Vector3.Lerp(colorStart, colorEnd, t);
                Vector4 finalColor = new Vector4(colorVec.X, colorVec.Y, colorVec.Z, 1.0f);

                var xpColorGradient = new ColorGradient();
                xpColorGradient.AddColor(0.0f, finalColor);
                xpColorGradient.AddColor(1.0f, finalColor);

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

            particleSystem.AddParticles(particles);
        }

        // New: Create XP orbs by XP value (auto-choose type)
        public static void CreateByXP(
            ParticleSystem particleSystem,
            int xpValue,
            Vector2 position,
            float attractDistance = 5.0f,
            float collectDistance = 0.3f,
            float maxAttractForce = 40.0f,
            float maxSpeed = 6.0f,
            float damping = 0.92f
        )
        {
            // Simple mapping: 1-10 = Small, 11-30 = Medium, 31-80 = Large, 81+ = Super
            if (xpValue <= 10)
                CreateByType(particleSystem, Math.Max(1, xpValue), position, XPOrbType.Small, attractDistance, collectDistance, maxAttractForce, maxSpeed, damping);
            else if (xpValue <= 30)
                CreateByType(particleSystem, Math.Max(1, xpValue / 2), position, XPOrbType.Medium, attractDistance, collectDistance, maxAttractForce, maxSpeed, damping);
            else if (xpValue <= 80)
                CreateByType(particleSystem, Math.Max(1, xpValue / 5), position, XPOrbType.Large, attractDistance, collectDistance, maxAttractForce, maxSpeed, damping);
            else
                CreateByType(particleSystem, Math.Max(1, xpValue / 10), position, XPOrbType.Super, attractDistance, collectDistance, maxAttractForce, maxSpeed, damping);
        }

        // Legacy: fallback for old calls
        public static void Create(
            ParticleSystem particleSystem,
            int amount,
            Vector2 position,
            float attractDistance = 5.0f,
            float collectDistance = 0.3f,
            float maxAttractForce = 40.0f,
            float maxSpeed = 6.0f,
            float damping = 0.92f
        )
        {
            CreateByType(particleSystem, amount, position, XPOrbType.Small, attractDistance, collectDistance, maxAttractForce, maxSpeed, damping);
        }
    }
}
