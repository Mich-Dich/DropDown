using OpenTK.Mathematics;
using System;
using Core.Particles;
using Core.util;

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
                        // Get player velocity for prediction
                        Vector2 playerVelocity = util.convert_Vector<Vector2>(Game.Instance.player.Get_Velocity());
                        
                        // Predict where player will be in a short time (for better targeting)
                        Vector2 predictedPlayerPos = Game.Instance.player.transform.position + playerVelocity * deltaTime * 2.0f;
                        Vector2 toPredictedPlayer = predictedPlayerPos - Position;
                        
                        // Use a blend of current and predicted direction for smarter attraction
                        Vector2 direction = distance > 0 ? toPlayer / distance : Vector2.Zero;
                        Vector2 predictedDirection = toPredictedPlayer.Length > 0 ? toPredictedPlayer / toPredictedPlayer.Length : Vector2.Zero;
                        
                        // Blend directions based on distance (more prediction when further away)
                        float predictionBlend = Math.Clamp(distance / AttractDistance, 0.0f, 0.7f);
                        Vector2 finalDirection = Vector2.Lerp(direction, predictedDirection, predictionBlend);

                        // Check if particle is moving away from player (negative dot product)
                        float velocityDotDirection = Vector2.Dot(Velocity, finalDirection);
                        bool isMovingAway = velocityDotDirection < 0;

                        // Improved force calculation: stronger attraction as particle gets closer
                        float forceFactor = 1.0f + (1.0f - (distance / AttractDistance)) * 2.0f;
                        forceFactor = Math.Clamp(forceFactor, 0.5f, 3.0f);
                        
                        // Add extra force when very close to prevent drifting
                        if (distance < AttractDistance * 0.3f)
                        {
                            forceFactor *= 2.0f; // Double force when very close
                        }

                        // Momentum correction: if moving away, apply stronger correction force
                        if (isMovingAway)
                        {
                            // Calculate how much the particle is moving away
                            float awaySpeed = Math.Abs(velocityDotDirection);
                            
                            // Apply stronger correction force when moving away
                            float correctionFactor = Math.Clamp(awaySpeed / MaxSpeed, 0.5f, 2.0f);
                            forceFactor *= (1.0f + correctionFactor);
                            
                            // Also reduce the particle's speed when moving away to help it turn
                            if (Velocity.Length > MaxSpeed * 0.5f)
                            {
                                Velocity *= 0.95f; // Gradually slow down when moving away
                            }
                        }
                        
                        float forceMagnitude = MaxAttractForce * forceFactor;
                        Velocity += finalDirection * forceMagnitude * deltaTime;

                        // Smart damping: less damping when moving toward player, more when moving away
                        float dampingFactor = isMovingAway ? 0.85f : 0.92f;
                        Velocity *= MathF.Pow(dampingFactor, deltaTime * 0.5f);

                        // Cap speed but allow higher speeds when very close
                        float currentMaxSpeed = distance < AttractDistance * 0.2f ? MaxSpeed * 1.5f : MaxSpeed;
                        if (Velocity.Length > currentMaxSpeed)
                            Velocity = Velocity.Normalized() * currentMaxSpeed;
                    }
                    else
                    {
                        // Inside CollectDistance => collect immediately
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
