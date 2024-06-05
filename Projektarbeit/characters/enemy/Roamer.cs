namespace Hell.enemy
{
    using Box2DX.Common;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Hell.weapon;
    using OpenTK.Mathematics;

    public class Roamer : CH_base_NPC
    {
        /*
        Roams in front of the player and attacks when the player is in range.
        Keeps distance to player.
        Approaches player when too far away.
        Retreats when too close.
        Bouncy movement.
        */

        private readonly Random random = new ();
        private readonly float movementOffset = 0f;

        public Roamer()
            : base()
        {
            this.auto_remove_on_death = true;
            this.damage = 10;
            this.movement_speed = 300;
            this.attackRange = 400;
            this.shootInterval = 1.0f;

            this.movementOffset = (float)this.random.NextDouble() * 5;

            this.attackAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            this.walkAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            this.idleAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            this.hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public float PlayerDistance
        {
            get
            {
                Vector2 playerPosition = Game.Instance.player.transform.position;
                return (playerPosition - this.transform.position).Length;
            }
        }

        public void Move(Vector2 direction)
        {
            direction.NormalizeFast();
            Vec2 dir = new (direction.X, direction.Y);
            this.Add_Linear_Velocity(dir * this.movement_speed * Game_Time.delta);
        }

        public void Roam()
        {
            Vector2 direction = new (MathF.Sin(Game_Time.total + this.movementOffset), -MathF.Cos(Game_Time.total + this.movementOffset));
            this.Move(direction);
        }

        public void Fire(Vector2 direction)
        {
            if (Game_Time.total - this.lastShootTime > this.shootInterval)
            {
                direction.NormalizeFast();
                Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(this.transform.position, direction));
                this.lastShootTime = Game_Time.total;
            }
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile testProjectile)
            {
                this.apply_damage(testProjectile.Damage);
                this.set_animation_from_anim_data(this.hitAnim);
            }

            if (hit.hit_object is IReflectable projectile)
            {
                if (projectile.Reflected)
                {
                    this.apply_damage(projectile.Damage * 3);
                    this.set_animation_from_anim_data(this.hitAnim);
                }
            }

            if (this.health <= 0)
            {
                this.IsDead = true;
            }
        }

        public override bool IsPlayerInRange()
        {
            throw new NotImplementedException();
        }

        public override bool IsPlayerInAttackRange()
        {
            throw new NotImplementedException();
        }

        public override bool IsHealthLow()
        {
            throw new NotImplementedException();
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }

        public override void Pursue()
        {
            throw new NotImplementedException();
        }

        public override void Attack()
        {
            throw new NotImplementedException();
        }

        public override void Retreat()
        {
            throw new NotImplementedException();
        }
    }
}