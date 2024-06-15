namespace Projektarbeit.characters.enemy.character
{
    using Box2DX.Common;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class Roamer : CH_base_NPC
    {
        /*
        Roams in front of the player and attacks when the player is in range.
        Keeps distance to player.
        Approaches player when too far away.
        Retreats when too close.
        Bouncy movement.
        */

        private readonly Random random = new();
        private readonly float movementOffset = 0f;

        public Roamer()
            : base()
        {
            auto_remove_on_death = true;
            damage = 10;
            movement_speed = 300;
            attackRange = 400;
            shootInterval = 1.0f;

            movementOffset = (float)random.NextDouble() * 5;

            attackAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walkAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public float PlayerDistance
        {
            get
            {
                Vector2 playerPosition = Core.Game.Instance.player.transform.position;
                return (playerPosition - transform.position).Length;
            }
        }

        public void Move(Vector2 direction)
        {
            direction.NormalizeFast();
            Vec2 dir = new(direction.X, direction.Y);
            Add_Linear_Velocity(dir * movement_speed * Game_Time.delta);
        }

        public void Roam()
        {
            Vector2 direction = new(MathF.Sin(Game_Time.total + movementOffset), -MathF.Cos(Game_Time.total + movementOffset));
            Move(direction);
        }

        public void Fire(Vector2 direction)
        {
            if (Game_Time.total - lastShootTime > shootInterval)
            {
                direction.NormalizeFast();
                Core.Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(transform.position, direction));
                lastShootTime = Game_Time.total;
            }
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile testProjectile)
            {
                apply_damage(testProjectile.Damage);
                set_animation_from_anim_data(hitAnim);
            }

            if (hit.hit_object is IReflectable projectile)
            {
                if (projectile.Reflected)
                {
                    apply_damage(projectile.Damage * 3);
                    set_animation_from_anim_data(hitAnim);
                }
            }

            if (health <= 0)
            {
                IsDead = true;
            }
        }

    }
}