namespace Projektarbeit.characters.enemy.character
{
    using Core.defaults;
    using Core.world;
    using Core.util;
    using Core.physics;
    using Core.render;
    using Core.Controllers.ai;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;
    using System;
    using System.Threading.Tasks;

    public class Boss : CH_base_NPC
    {
        private readonly Random random = new();
        private float phaseTransitionTime = 0f;
        private float lastSpecialAttackTime = 0f;
        private float lastMultiShotTime = 0f;
        private float lastMortarTime = 0f;
        private int currentPhase = 1;
        private bool isPerformingSpecialAttack = false;
        
        // Staggered attack system
        private bool isStaggeredAttackActive = false;
        private int staggeredAttackStep = 0;
        private float staggeredAttackTimer = 0f;
        private const float STAGGERED_ATTACK_DELAY = 0.08f; // Faster projectiles (80ms between projectiles)
        
        // Performance optimization - limit active projectiles
        private const int MAX_ACTIVE_PROJECTILES = 25; // Increased limit for more intense fights
        private static int activeProjectileCount = 0;

        // Attack patterns timing - Made more aggressive
        private const float SPECIAL_ATTACK_COOLDOWN = 5f; // Reduced from 8f
        private const float MULTI_SHOT_COOLDOWN = 2.5f; // Reduced from 4f
        private const float MORTAR_COOLDOWN = 4f; // Reduced from 6f
        private const float RAPID_FIRE_DURATION = 3f;
        
        // Movement patterns
        private Vector2 orbitCenter;
        private float orbitAngle = 0f;
        private float orbitRadius = 300f;
        private bool orbitingClockwise = true;

        public Boss(AI_Controller controller)
            : base()
        {
            XPValue = 500; // Much higher XP reward for boss
            Controller = controller;
            transform.size = new Vector2(200); // Much larger boss size (was 120)
            health_max = 2500; // Increased health for longer fight
            health = health_max;

            movement_speed = 120; // Much faster movement speed for aggressive chasing
            movement_speed_max = 150; // Much higher max speed for intense pursuit
            rotation_offset = float.Pi / 2;

            damage = 25; // Increased damage
            rayNumber = 20;
            rayCastRange = 1000;
            rayCastAngle = float.Pi;
            autoDetectionRange = 800;
            attackRange = 700; // Increased attack range
            
            // Override base class pursuit settings for more aggressive boss behavior
            StopDistance = 150f; // Closer stop distance for more aggressive pursuit
            PursueSpeed = 150; // Much faster pursuit speed

            lastShootTime = 0f;
            shootInterval = 0.6f; // Faster shooting
            fireDelay = 1.2f; // Reduced fire delay

            // Use static texture instead of animation for testing
            // Use the first frame of the boss sprite sheet (0001.png)
            var bossTexture = Resource_Manager.Get_Texture("assets/animation/enemy/boss/0001.png");
            var staticSprite = new Sprite(bossTexture);
            
            // Flip the sprite horizontally by manually adjusting UV coordinates
            // The sprite uses UV coordinates where (0,0) is bottom-left and (1,1) is top-right
            // To flip horizontally, we swap the X UV coordinates
            staticSprite.Select_Texture_Region(1, 1, 0, 0); // This will use the full texture
            // Note: The actual flipping will be handled by the rotation in the boss movement
            
            Set_Sprite(staticSprite);
            
            // Create empty animations for testing (no animation)
            attackAnim = new animation_data("", 1, 1, false, false, 1, false);
            walkAnim = new animation_data("", 1, 1, false, false, 1, false);
            idleAnim = new animation_data("", 1, 1, false, false, 1, false);
            hitAnim = new animation_data("", 1, 1, false, false, 1, false);
            
            orbitCenter = transform.position;
        }

        public override void Update(float deltaTime)
        {
            // Don't update if dead - immediate return
            if (IsDead || health <= 0)
            {
                return;
            }
            
            base.Update(deltaTime);
            
            UpdatePhase();
            UpdateOrbitMovement();
            
            // Check for special attacks
            if (CanPerformSpecialAttack())
            {
                PerformSpecialAttack();
            }
            
            // Update staggered attack
            if (isStaggeredAttackActive)
            {
                staggeredAttackTimer += deltaTime;
                if (staggeredAttackTimer >= STAGGERED_ATTACK_DELAY)
                {
                    ExecuteStaggeredAttackStep();
                    staggeredAttackTimer = 0f;
                }
            }
        }

        public override void Attack()
        {
            // Don't attack if dead - immediate return
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // Don't attack if performing special attack
            if (isPerformingSpecialAttack)
            {
                return;
            }
            
            switch (currentPhase)
            {
                case 1:
                    Phase1Attack();
                    break;
                case 2:
                    Phase2Attack();
                    break;
                case 3:
                    Phase3Attack();
                    break;
            }
        }

        private void UpdatePhase()
        {
            float healthPercentage = health / health_max;
            
            if (healthPercentage <= 0.3f && currentPhase != 3)
            {
                OnPhaseTransition(3);
            }
            else if (healthPercentage <= 0.6f && currentPhase != 2)
            {
                OnPhaseTransition(2);
            }
        }

        private void OnPhaseTransition(int phase)
        {
            if (currentPhase == phase) return;
            
            currentPhase = phase;
            Console.WriteLine($"Boss entering phase {phase}!");
            
            // Trigger camera shake for phase transition
            Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
            
            // Reset attack timers for new phase
            lastShootTime = 0f;
            lastSpecialAttackTime = 0f;
            lastMultiShotTime = 0f;
            lastMortarTime = 0f;
        }

        private void UpdateOrbitMovement()
        {
            // Don't move if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - transform.position).Length;
            
            // Orbit around player when in range
            if (distanceToPlayer < 800f)
            {
                orbitAngle += (orbitingClockwise ? 1 : -1) * 0.05f; // Increased orbit speed from 0.02f to 0.05f
                Vector2 orbitOffset = new Vector2(
                    MathF.Cos(orbitAngle) * orbitRadius,
                    MathF.Sin(orbitAngle) * orbitRadius
                );
                
                Vector2 targetPosition = playerPosition + orbitOffset;
                Vector2 direction = (targetPosition - transform.position).Normalized();
                
                // Apply movement force
                Box2DX.Common.Vec2 force = new Box2DX.Common.Vec2(direction.X, direction.Y) * movement_speed * 1000f;
                if (collider != null && collider.body != null)
                {
                    collider.body.ApplyForce(force, collider.body.GetWorldCenter());
                }
                
                // Rotate to face player
                rotate_to_vector_smooth(playerPosition - transform.position);
            }
            else
            {
                // Move towards player if too far
                Vector2 direction = (playerPosition - transform.position).Normalized();
                Box2DX.Common.Vec2 force = new Box2DX.Common.Vec2(direction.X, direction.Y) * movement_speed * 1000f;
                if (collider != null && collider.body != null)
                {
                    collider.body.ApplyForce(force, collider.body.GetWorldCenter());
                }
            }
        }

        private bool CanPerformSpecialAttack()
        {
            return Game_Time.total - lastSpecialAttackTime >= SPECIAL_ATTACK_COOLDOWN && !isPerformingSpecialAttack;
        }

        private void PerformSpecialAttack()
        {
            // Don't perform special attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            lastSpecialAttackTime = Game_Time.total;
            isPerformingSpecialAttack = true;
            
            switch (currentPhase)
            {
                case 1:
                    MultiDirectionalAttack();
                    break;
                case 2:
                    ExplosiveBombardment();
                    break;
                case 3:
                    RapidFireBarrage();
                    break;
            }
            
            // End special attack after a short duration
            Task.Delay(2000).ContinueWith(_ => isPerformingSpecialAttack = false);
        }

        private void Phase1Attack()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // More aggressive basic attacks with variety of projectiles (no sparkles for performance)
            if (Game_Time.total - lastShootTime >= fireDelay * 0.8f) // Faster firing
            {
                Vector2 direction = GetDirectionToPlayer();
                
                // Fire multiple projectiles more frequently (no sparkles)
                if (random.NextDouble() < 0.6)
                {
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                }
                else
                {
                    // Sometimes fire multiple projectiles
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                    CreateProjectileSafely<EnemyTestProjectile>(transform.position, direction);
                }
                
                lastShootTime = Game_Time.total;
                set_animation_from_anim_data(attackAnim);
            }
        }

        private void Phase2Attack()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // More aggressive mix of different projectile types (no sparkles)
            if (Game_Time.total - lastShootTime >= fireDelay * 0.5f) // Much faster firing
            {
                if (random.NextDouble() < 0.4)
                {
                    // Regular sniper shot
                    Vector2 direction = GetDirectionToPlayer();
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                }
                else if (random.NextDouble() < 0.7)
                {
                    // Explosive projectile
                    Vector2 direction = GetDirectionToPlayer();
                    CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
                }
                else
                {
                    // Fire both types for more intensity
                    Vector2 direction = GetDirectionToPlayer();
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                    CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
                }
                
                lastShootTime = Game_Time.total;
                set_animation_from_anim_data(attackAnim);
            }
            
            // More frequent mortar attacks
            if (Game_Time.total - lastMortarTime >= MORTAR_COOLDOWN * 0.8f)
            {
                FireMortar();
                lastMortarTime = Game_Time.total;
            }
        }

        private void Phase3Attack()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // Very aggressive rapid fire attacks (no sparkles)
            if (Game_Time.total - lastShootTime >= fireDelay * 0.3f) // Very fast firing
            {
                Vector2 direction = GetDirectionToPlayer();
                
                // Fire multiple projectiles rapidly
                if (random.NextDouble() < 0.3)
                {
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                }
                else if (random.NextDouble() < 0.6)
                {
                    CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
                }
                else if (random.NextDouble() < 0.8)
                {
                    CreateProjectileSafely<EnemyTestProjectile>(transform.position, direction);
                }
                else
                {
                    // Fire multiple projectiles at once for maximum intensity
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                    CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
                }
                
                lastShootTime = Game_Time.total;
                set_animation_from_anim_data(attackAnim);
            }
            
            // Very frequent special attacks
            if (Game_Time.total - lastMultiShotTime >= MULTI_SHOT_COOLDOWN * 0.7f)
            {
                MultiDirectionalAttack();
                lastMultiShotTime = Game_Time.total;
            }
        }

        private void MultiDirectionalAttack()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // Start staggered attack instead of firing all at once
            isStaggeredAttackActive = true;
            staggeredAttackStep = 0;
            staggeredAttackTimer = 0f;
            
            // Trigger camera shake for impact
            Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
        }
        
        private void ExecuteStaggeredAttackStep()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                isStaggeredAttackActive = false;
                staggeredAttackStep = 0;
                return;
            }
            
            if (staggeredAttackStep < 6) // Increased from 4 to 6 projectiles
            {
                float angle = (staggeredAttackStep * MathF.PI * 2) / 6;
                Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                
                // Use simpler projectile for better performance (no sparkles)
                CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                
                staggeredAttackStep++;
            }
            else
            {
                // End staggered attack
                isStaggeredAttackActive = false;
                staggeredAttackStep = 0;
            }
        }

        private void ExplosiveBombardment()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // Fire 3 explosive projectiles for more intensity
            Vector2 baseDirection = GetDirectionToPlayer();
            
            for (int i = -1; i <= 1; i++) // Fire 3 projectiles
            {
                float angleOffset = i * 0.3f; // Tighter spread
                Vector2 direction = RotateVector(baseDirection, angleOffset);
                
                CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
            }
            
            Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
        }

        private void RapidFireBarrage()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // Fire 3 projectiles for more intensity
            for (int i = 0; i < 3; i++)
            {
                Vector2 direction = GetDirectionToPlayer();
                // Add some spread to make it dodgeable
                float spread = (float)(random.NextDouble() - 0.5) * 0.4f; // Increased spread
                direction = RotateVector(direction, spread);
                
                CreateProjectileSafely<SniperProjectile>(transform.position, direction);
            }
        }

        private void FireMortar()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            Vector2 direction = GetDirectionToPlayer();
            CreateProjectileSafely<MortarProjectile>(transform.position, direction);
        }
        
        private void CreateProjectileSafely<T>(Vector2 position, Vector2 direction) where T : Game_Object
        {
            // Don't create projectiles if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // Check if game instance and active map are available
            if (Game.Instance == null || Game.Instance.get_active_map() == null)
            {
                Console.WriteLine("Game instance or active map is null, cannot create projectile");
                return;
            }
            
            // Check if physics world is available
            if (Game.Instance.get_active_map().physicsWorld == null)
            {
                Console.WriteLine("Physics world is null, cannot create projectile");
                return;
            }
            
            // Check if we can create more projectiles
            if (activeProjectileCount >= MAX_ACTIVE_PROJECTILES)
            {
                return; // Skip creating this projectile to prevent lag
            }
            
            try
            {
                Game_Object projectile = null;
                
                // Create the appropriate projectile type with proper error handling (no sparkles)
                if (typeof(T) == typeof(SniperProjectile))
                {
                    projectile = new SniperProjectile(position, direction);
                }
                else if (typeof(T) == typeof(ExplosivProjectile))
                {
                    projectile = new ExplosivProjectile(position, direction);
                }
                else if (typeof(T) == typeof(MortarProjectile))
                {
                    projectile = new MortarProjectile(position, direction);
                }
                else if (typeof(T) == typeof(EnemyTestProjectile))
                {
                    projectile = new EnemyTestProjectile(position, direction);
                }
                
                if (projectile != null)
                {
                    // Add projectile to the map
                    Game.Instance.get_active_map().Add_Game_Object(projectile);
                    activeProjectileCount++;
                    
                    // Track projectile for cleanup
                    Task.Delay(5000).ContinueWith(_ =>
                    {
                        if (activeProjectileCount > 0)
                            activeProjectileCount--;
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create projectile: {ex.Message}");
                // Don't increment activeProjectileCount on failure
            }
        }

        public override void Hit(hitData hit)
        {
            // Don't process hits if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            base.Hit(hit);
            
            // Flash red when hit
            if (sprite != null)
            {
                sprite.TintColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); // Red tint
                
                // Reset tint after a short delay
                Task.Delay(100).ContinueWith(_ =>
                {
                    if (sprite != null && !IsDead)
                    {
                        sprite.TintColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); // Reset to white
                    }
                });
            }
            
            // Counter-attack chance
            if (random.NextDouble() < 0.3f) // 30% chance
            {
                CounterAttack();
            }
        }

        private void CounterAttack()
        {
            // Don't attack if dead
            if (IsDead || health <= 0)
            {
                return;
            }
            
            // More aggressive counter-attack - fire 3 projectiles (no sparkles)
            Vector2 baseDirection = GetDirectionToPlayer();
            
            for (int i = -1; i <= 1; i++) // Fire 3 projectiles
            {
                Vector2 direction = RotateVector(baseDirection, i * 0.15f); // Tighter spread
                CreateProjectileSafely<SniperProjectile>(transform.position, direction);
            }
        }
    }
}
