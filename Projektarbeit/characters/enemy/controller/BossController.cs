using OpenTK.Mathematics;
using Core.Controllers.ai;
using Core.world;
using Core.util;
using Projektarbeit.characters.enemy.character;
using Projektarbeit.characters.enemy.States;
using Projektarbeit.particles;

namespace Projektarbeit.characters.enemy.controller
{
    public class BossController : AI_Controller
    {
        public BossController(Vector2 origin)
            : base(new List<Character>())
        {
            CH_base_NPC boss = new Boss(this);
            characters.Add(boss);
            Game.Instance.get_active_map().Add_Character(boss, origin, 0, true);
            
            // Add death callback for wave progress tracking and dramatic death effects
            boss.death_callback = () =>
            {
                BossDeathEffects(boss.transform.position);
                
                // XP drop (super orbs for bosses - more rewarding)
                XPParticleEffect.CreateByType(
                    Core.Game.Instance.get_active_map().particleSystem,
                    15, // More XP orbs for boss
                    boss.transform.position,
                    XPOrbType.Super,
                    attractDistance: 400.0f,
                    collectDistance: 60.0f,
                    maxAttractForce: 600.0f,
                    maxSpeed: 500.0f,
                    damping: 0.95f
                );
                
                var currentWave = Projektarbeit.Levels.Wave.GetCurrentWave();
                currentWave?.EnemyDefeated();
                
                Console.WriteLine("BOSS DEFEATED! Wave complete!");
            };
            
            get_state_machine().Set_Statup_State(typeof(Pursue));
        }
        
        private void BossDeathEffects(Vector2 bossPosition)
        {
            // Create massive camera shake for boss death
            var earthquakeShake = new ShakeProfile(2.5f, 0.85f, 15f); // Intense shake
            Game.Instance.camera.transform.ApplyShake(earthquakeShake);
            
            // Create multiple explosion effects in sequence
            CreateExplosionSequence(bossPosition);
            
            // Create shockwave effect
            CreateShockwaveEffect(bossPosition);
            
            // Additional particle effects for dramatic flair
            CreateDeathParticleEffects(bossPosition);
        }
        
        private void CreateExplosionSequence(Vector2 centerPosition)
        {
            // Create multiple explosions with slight delays for dramatic effect
            Task.Run(async () =>
            {
                var random = new Random();
                
                // Main central explosion
                CreateSingleExplosion(centerPosition, 200f);
                
                await Task.Delay(200);
                
                // Secondary explosions around the boss
                for (int i = 0; i < 8; i++)
                {
                    float angle = (i * MathF.PI * 2) / 8;
                    float distance = 100f + (float)random.NextDouble() * 100f;
                    Vector2 explosionPos = centerPosition + new Vector2(
                        MathF.Cos(angle) * distance,
                        MathF.Sin(angle) * distance
                    );
                    
                    CreateSingleExplosion(explosionPos, 120f);
                    
                    // Small delay between explosions
                    await Task.Delay(100);
                }
                
                await Task.Delay(300);
                
                // Final massive explosion
                CreateSingleExplosion(centerPosition, 300f);
                
                // Final camera shake
                var finalShake = new ShakeProfile(1.8f, 0.9f, 25f);
                Game.Instance.camera.transform.ApplyShake(finalShake);
            });
        }
        
        private void CreateSingleExplosion(Vector2 position, float size)
        {
            // Create explosion effect with camera shake
            Game.Instance.camera.transform.ApplyShake(CameraShake.Explosion);
            Console.WriteLine($"[Boss] Explosion at {position} with size {size}");
        }
        
        private void CreateShockwaveEffect(Vector2 centerPosition)
        {
            // Create shockwave effect with screen shake
            var shockwaveShake = new ShakeProfile(1.5f, 0.9f, 20f);
            Game.Instance.camera.transform.ApplyShake(shockwaveShake);
            Console.WriteLine($"[Boss] Shockwave effect at {centerPosition}");
        }
        
        private void CreateDeathParticleEffects(Vector2 centerPosition)
        {
            // Create death particle effects with screen shake
            var deathShake = new ShakeProfile(1.2f, 0.85f, 18f);
            Game.Instance.camera.transform.ApplyShake(deathShake);
            Console.WriteLine($"[Boss] Death particle effects at {centerPosition}");
        }
    }   
}