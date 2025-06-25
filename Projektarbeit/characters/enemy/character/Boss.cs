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
        private const float STAGGERED_ATTACK_DELAY = 0.1f; // 100ms between projectiles
        
        // Performance optimization - limit active projectiles
        private const int MAX_ACTIVE_PROJECTILES = 20;
        private static int activeProjectileCount = 0;

        // Attack patterns timing
        private const float SPECIAL_ATTACK_COOLDOWN = 8f;
        private const float MULTI_SHOT_COOLDOWN = 4f;
        private const float MORTAR_COOLDOWN = 6f;
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
            transform.size = new Vector2(120); // Larger boss size
            health_max = 2000; // Much higher health for engaging boss fight
            health = health_max;

            movement_speed = 25;
            movement_speed_max = 35;
            rotation_offset = float.Pi / 2;

            damage = 15; // Higher damage
            rayNumber = 20;
            rayCastRange = 1000;
            rayCastAngle = float.Pi;
            autoDetectionRange = 800;
            attackRange = 600; // Longer attack range

            lastShootTime = 0f;
            shootInterval = 0.8f;
            fireDelay = 1.5f;

            // Using new 80-frame boss animation (15 fps, looping)
            // Note: This assumes the individual frames have been combined into a sprite sheet
            // with 10 columns and 8 rows (10x8 = 80 frames)
            var bossAnimPath = "DropDown/assets/animation/boss/boss_spritesheet.png";
            
            // Check if custom boss sprite sheet exists, otherwise use placeholder
            if (!System.IO.File.Exists(bossAnimPath))
            {
                // Fallback to existing animation until sprite sheet is created
                Console.WriteLine("[Boss] Using fallback animation - boss sprite sheet not found");
                bossAnimPath = "assets/animation/enemy/CrystalKnightIdle.png";
                attackAnim = new animation_data(bossAnimPath, 4, 1, true, true, 8, false);
                walkAnim = new animation_data(bossAnimPath, 4, 1, true, true, 15, true);
                idleAnim = new animation_data(bossAnimPath, 4, 1, true, true, 15, true);
                hitAnim = new animation_data("assets/animation/enemy/CrystalKnightHit.png", 1, 1, true, true, 3, false);
            }
            else
            {
                // Use the new 80-frame boss animation
                Console.WriteLine("[Boss] Using new 80-frame boss animation");
                attackAnim = new animation_data(bossAnimPath, 8, 10, true, true, 12, false); // Slightly slower for attack
                walkAnim = new animation_data(bossAnimPath, 8, 10, true, true, 15, true);   // 15 fps as requested
                idleAnim = new animation_data(bossAnimPath, 8, 10, true, true, 15, true);   // 15 fps as requested  
                hitAnim = new animation_data(bossAnimPath, 8, 10, true, true, 20, false);   // Faster for hit reaction
            }
            
            orbitCenter = transform.position;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            // Update staggered attack system
            if (isStaggeredAttackActive)
            {
                staggeredAttackTimer += deltaTime;
                if (staggeredAttackTimer >= STAGGERED_ATTACK_DELAY)
                {
                    staggeredAttackTimer = 0f;
                    ExecuteStaggeredAttackStep();
                }
            }
            
            UpdatePhase();
            UpdateOrbitMovement();
            
            // Check for special attacks
            if (CanPerformSpecialAttack())
            {
                PerformSpecialAttack();
            }
        }

        public override void Attack()
        {
            // Regular attack pattern based on phase
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
            float healthPercentage = (float)health / health_max;
            int newPhase = healthPercentage switch
            {
                > 0.66f => 1,
                > 0.33f => 2,
                _ => 3
            };

            if (newPhase != currentPhase)
            {
                currentPhase = newPhase;
                phaseTransitionTime = Game_Time.total;
                OnPhaseTransition(newPhase);
            }
        }

        private void OnPhaseTransition(int phase)
        {
            // Trigger camera shake for phase transitions
            Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
            
            // Reset special attack timers on phase change
            lastSpecialAttackTime = Game_Time.total;
            lastMultiShotTime = Game_Time.total;
            lastMortarTime = Game_Time.total;
            
            // Change orbit direction
            orbitingClockwise = !orbitingClockwise;
            orbitRadius = phase * 100f + 200f; // Increase orbit radius per phase
            
            Console.WriteLine($"Boss entered Phase {phase}!");
        }

        private void UpdateOrbitMovement()
        {
            // Update orbit center to player position with some lag for predictability
            Vector2 playerPos = Game.Instance.player.transform.position;
            orbitCenter = Vector2.Lerp(orbitCenter, playerPos, 0.02f);
            
            // Orbit around the player
            float orbitSpeed = currentPhase * 0.8f + 1.2f; // Faster orbiting in later phases
            orbitAngle += (orbitingClockwise ? 1 : -1) * orbitSpeed * Game_Time.delta;
            
            Vector2 targetPosition = orbitCenter + new Vector2(
                MathF.Cos(orbitAngle) * orbitRadius,
                MathF.Sin(orbitAngle) * orbitRadius
            );
            
            Vector2 direction = (targetPosition - transform.position).Normalized();
            ApplyForceInDirection(direction, movement_speed * currentPhase);
        }

        private bool CanPerformSpecialAttack()
        {
            return Game_Time.total - lastSpecialAttackTime >= SPECIAL_ATTACK_COOLDOWN && !isPerformingSpecialAttack;
        }

        private void PerformSpecialAttack()
        {
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
            // Basic attacks with variety of projectiles
            if (Game_Time.total - lastShootTime >= fireDelay)
            {
                Vector2 direction = GetDirectionToPlayer();
                
                // Alternate between different projectile types for variety
                if (random.NextDouble() < 0.6)
                {
                    CreateProjectileSafely<SparkProjectile>(transform.position, direction);
                }
                else
                {
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                }
                
                lastShootTime = Game_Time.total;
                set_animation_from_anim_data(attackAnim);
            }
        }

        private void Phase2Attack()
        {
            // Mix of different projectile types
            if (Game_Time.total - lastShootTime >= fireDelay * 0.7f)
            {
                if (random.NextDouble() < 0.6)
                {
                    // Regular sniper shot
                    Vector2 direction = GetDirectionToPlayer();
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                }
                else
                {
                    // Explosive projectile
                    Vector2 direction = GetDirectionToPlayer();
                    CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
                }
                
                lastShootTime = Game_Time.total;
                set_animation_from_anim_data(attackAnim);
            }
            
            // Mortar attacks
            if (Game_Time.total - lastMortarTime >= MORTAR_COOLDOWN)
            {
                FireMortar();
                lastMortarTime = Game_Time.total;
            }
        }

        private void Phase3Attack()
        {
            // Rapid fire attacks
            if (Game_Time.total - lastShootTime >= fireDelay * 0.4f)
            {
                Vector2 direction = GetDirectionToPlayer();
                
                // Alternate between different projectile types rapidly
                if (random.NextDouble() < 0.4)
                {
                    CreateProjectileSafely<SniperProjectile>(transform.position, direction);
                }
                else if (random.NextDouble() < 0.7)
                {
                    CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
                }
                else
                {
                    CreateProjectileSafely<EnemyTestProjectile>(transform.position, direction);
                }
                
                lastShootTime = Game_Time.total;
                set_animation_from_anim_data(attackAnim);
            }
            
            // More frequent special attacks
            if (Game_Time.total - lastMultiShotTime >= MULTI_SHOT_COOLDOWN)
            {
                MultiDirectionalAttack();
                lastMultiShotTime = Game_Time.total;
            }
        }

        private void MultiDirectionalAttack()
        {
            // Start staggered attack instead of firing all at once
            isStaggeredAttackActive = true;
            staggeredAttackStep = 0;
            staggeredAttackTimer = 0f;
            
            // Trigger camera shake for impact
            Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
        }
        
        private void ExecuteStaggeredAttackStep()
        {
            if (staggeredAttackStep < 4) // Fire 4 projectiles total
            {
                float angle = (staggeredAttackStep * MathF.PI * 2) / 4;
                Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                
                // Use simpler projectile for better performance
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
            // Fire 2 explosive projectiles instead of 3 to reduce lag
            Vector2 baseDirection = GetDirectionToPlayer();
            
            for (int i = -1; i <= 1; i += 2) // Only fire 2 projectiles
            {
                float angleOffset = i * 0.4f; // Spread the projectiles
                Vector2 direction = RotateVector(baseDirection, angleOffset);
                
                CreateProjectileSafely<ExplosivProjectile>(transform.position, direction);
            }
            
            Game.Instance.camera.transform.ApplyShake(CameraShake.LargeProjectileHit);
        }

        private void RapidFireBarrage()
        {
            // Fire 2 projectiles instead of 3 to reduce lag
            for (int i = 0; i < 2; i++)
            {
                Vector2 direction = GetDirectionToPlayer();
                // Add some spread to make it dodgeable
                float spread = (float)(random.NextDouble() - 0.5) * 0.3f;
                direction = RotateVector(direction, spread);
                
                CreateProjectileSafely<SniperProjectile>(transform.position, direction);
            }
        }

        private void FireMortar()
        {
            Vector2 direction = GetDirectionToPlayer();
            CreateProjectileSafely<MortarProjectile>(transform.position, direction);
        }
        
        private void CreateProjectileSafely<T>(Vector2 position, Vector2 direction) where T : Game_Object
        {
            // Check if we can create more projectiles
            if (activeProjectileCount >= MAX_ACTIVE_PROJECTILES)
            {
                return; // Skip creating this projectile to prevent lag
            }
            
            try
            {
                Game_Object projectile = null;
                
                // Create the appropriate projectile type
                if (typeof(T) == typeof(SniperProjectile))
                {
                    projectile = new SniperProjectile(position, direction);
                }
                else if (typeof(T) == typeof(ExplosivProjectile))
                {
                    projectile = new ExplosivProjectile(position, direction);
                }
                else if (typeof(T) == typeof(SparkProjectile))
                {
                    projectile = new SparkProjectile(position, direction);
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
                    Game.Instance.get_active_map().Add_Game_Object(projectile);
                    activeProjectileCount++;
                    
                    // Schedule cleanup when projectile is destroyed
                    Task.Delay(5000).ContinueWith(_ => {
                        if (activeProjectileCount > 0)
                            activeProjectileCount--;
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create projectile: {ex.Message}");
            }
        }

        public override void Hit(hitData hit)
        {
            base.Hit(hit);
            
            // Chance to trigger counter-attack when hit
            if (random.NextDouble() < 0.3) // 30% chance
            {
                CounterAttack();
            }
        }

        private void CounterAttack()
        {
            // Quick multi-shot as counter-attack - reduced to 2 projectiles
            Vector2 baseDirection = GetDirectionToPlayer();
            
            for (int i = -1; i <= 1; i += 2) // Only fire 2 projectiles
            {
                Vector2 direction = RotateVector(baseDirection, i * 0.2f);
                CreateProjectileSafely<SniperProjectile>(transform.position, direction);
            }
        }
    }
}