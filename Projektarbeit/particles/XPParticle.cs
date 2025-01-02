using OpenTK.Mathematics;
using System;
using Core.Particles;

namespace Projektarbeit.particles
{
    public enum XPParticleState
    {
        Waiting,   // Not yet close enough to be attracted
        Attracted, // Moves toward the player
        Collected  // Marked for removal
    }

    public class XPParticle : Core.Particles.Particle
    {
        public XPParticleState State { get; private set; } = XPParticleState.Waiting;

        // XP Particle parameters
        public float AttractDistance { get; set; } = 1.0f;   // The distance at which it becomes attracted
        public float CollectDistance { get; set; } = 0.2f;   // The distance at which it gets collected
        public float MaxAttractForce { get; set; } = 5.0f;   // The max force pulling it toward the player
        public float MaxSpeed { get; set; } = 2.0f;          // The speed limit
        public float Damping { get; set; } = 0.98f;          // Damping for smoother motion

        public XPParticle(
            Vector2 position, 
            float size,
            ColorGradient colorGradient,
            float attractDistance,
            float collectDistance,
            float maxAttractForce,
            float maxSpeed,
            float damping = 0.98f
        )
        : base(
            position: position,
            velocity: Vector2.Zero,
            size: size,
            rotation: 0f,
            lifeTime: 9999f, // Some large lifetime (or however long you want them to exist if not collected)
            colorGradient: colorGradient,
            isAffectedByForces: false
        )
        {
            AttractDistance = attractDistance;
            CollectDistance = collectDistance;
            MaxAttractForce = maxAttractForce;
            MaxSpeed = maxSpeed;
            Damping = damping;
        }

        public override void Update(Vector2 playerPosition, float deltaTime)
{
    Console.WriteLine("PlayerPos: " + Game.Instance.player.transform.position);

    Vector2 toPlayer = Game.Instance.player.transform.position - Position;
    float distance = toPlayer.Length;

    switch (State)
    {
        case XPParticleState.Waiting:
            if (distance <= AttractDistance)
            {
                State = XPParticleState.Attracted;
            }
            break;

        case XPParticleState.Attracted:
            if (distance > CollectDistance)
            {
                Vector2 direction = distance > 0 ? toPlayer / distance : Vector2.Zero;

                float forceFactor = 1.0f - (distance / AttractDistance);
                forceFactor = Math.Clamp(forceFactor, 0f, 1f);
                float forceMagnitude = MaxAttractForce * forceFactor;
                Velocity += direction * forceMagnitude * deltaTime;

                // Damping
                Velocity *= MathF.Pow(Damping, deltaTime);

                // Cap speed
                if (Velocity.Length > MaxSpeed)
                    Velocity = Velocity.Normalized() * MaxSpeed;
            }
            else
            {
                // Inside CollectDistance => kill
                State = XPParticleState.Collected;
            }
            break;

        case XPParticleState.Collected:
            Age = LifeTime; // forcibly kill
            break;
    }

    // Now call the base's logic to handle Age, Position, etc.
    base.Update(playerPosition, deltaTime);
}
    }
}
