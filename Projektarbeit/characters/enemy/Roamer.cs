
using Box2DX.Common;
using Core.physics;
using Core.render;
using Core.util;
using Hell.weapon;
using OpenTK.Mathematics;

namespace Hell.enemy {

    public class Roamer : CH_base_NPC {

        /*
        Roams in front of the player and attacks when the player is in range.
        Keeps distance to player.
        Approaches player when too far away.
        Retreats when too close.
        Bouncy movement.        
        */

        private Random random = new Random();
        private float movementOffset = 0f;

        public Roamer() : base() {
     
            auto_remove_on_death = true;
            damage = 10;
            movement_speed = 300;
            attack_range = 400;
            shoot_interval = 1.0f;

            movementOffset = (float)random.NextDouble() * 5;
            
            attack_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walk_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idle_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            hit_anim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public float PlayerDistance {
            get {
                Vector2 playerPosition = Game.Instance.player.transform.position;
                return (playerPosition - transform.position).Length;
            }
        }

        public void Move(Vector2 direction) {
            direction.NormalizeFast();
            Vec2 dir = new Vec2(direction.X, direction.Y);
            Add_Linear_Velocity(dir * movement_speed * Game_Time.delta);
        }

        public void Roam() {
            //unfinished
            Vector2 direction = new Vector2(MathF.Sin(Game_Time.total + movementOffset), -MathF.Cos(Game_Time.total + movementOffset));
            Move(direction);
        }

        public void Fire(Vector2 direction) {
            if (Game_Time.total - last_shoot_time > shoot_interval) {
                direction.NormalizeFast();
                Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(transform.position, direction));
                last_shoot_time = Game_Time.total;
            }
        }

        public override void Hit(hitData hit) {
            if(hit.hit_object is TestProjectile testProjectile) {
                this.apply_damage(testProjectile.Damage);
                set_animation_from_anim_data(hit_anim);
            }
            if(hit.hit_object is EnemyTestProjectile projectile) {
                if(projectile.Reflected) {
                    this.apply_damage(projectile.Damage * 3);
                    set_animation_from_anim_data(hit_anim);
                }
            }
            if(health <= 0) {
                IsDead = true;
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