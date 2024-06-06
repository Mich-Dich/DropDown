namespace Hell.enemy
{
    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Hell.weapon;
    using OpenTK.Mathematics;

    public class SwarmEnemy : CH_base_NPC
    {
        private const float StopDistance = 200f;
        private const float PursueSpeed = 60;

        private readonly Random random = new ();

        public SwarmEnemy()
            : base()
        {
            this.transform.size = new Vector2(40);
            this.movement_speed = 10;
            this.movement_speed_max = 20;
            this.rotation_offset = float.Pi / 2;

            this.attackAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            this.walkAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            this.idleAnim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            this.hitAnim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public override bool IsPlayerInRange()
        {
            if (Game.Instance.player == null || Game.Instance.player.IsDead)
            {
                return false;
            }

            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - this.transform.position).Length;
            return distanceToPlayer <= this.DetectionRange;
        }

        public override bool IsPlayerInAttackRange()
        {
            if (Game.Instance.player == null || Game.Instance.player.IsDead)
            {
                return false;
            }

            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - this.transform.position).Length;
            return distanceToPlayer <= StopDistance;
        }

        public override bool IsHealthLow()
        {
            return this.health <= this.health_max * 0.2;
        }

        public override void Move()
        {
            Vector2 direction = new (0, 1);
            Random random = new ();
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, 0);
            direction.NormalizeFast();
            direction *= this.movement_speed;
            this.movement_force = random.Next((int)this.movement_speed, (int)this.movement_speed_max);
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * this.movement_force * Game_Time.delta;
            if (velocity.Length() > this.movement_speed_max)
            {
                velocity.Normalize();
                velocity *= this.movement_speed_max;
            }

            this.Add_Linear_Velocity(velocity);
            this.rotate_to_vector_smooth(direction);
        }

        public override void Pursue()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = playerPosition - this.transform.position;

            if (direction.Length < StopDistance)
            {
                return;
            }

            direction.NormalizeFast();
            direction *= PursueSpeed;
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > this.movement_speed_max)
            {
                velocity.Normalize();
                velocity *= this.movement_speed_max;
            }

            this.Add_Linear_Velocity(velocity);
            this.rotate_to_vector_smooth(direction);

            this.ApplySeparation();
        }

        public override void Attack()
        {
            if (Game_Time.total - this.lastFireTime >= this.fireDelay + ((float)this.random.NextDouble() * (1f - 0.2f)) + 0.2f)
            {
            if (Game_Time.total - this.lastFireTime >= this.fireDelay)
            {
                Vector2 enemyLocation = this.transform.position;
                Vector2 playerPosition = Game.Instance.player.transform.position;
                Vector2 direction = (playerPosition - enemyLocation).Normalized();
                Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(enemyLocation, direction));
                this.lastFireTime = Game_Time.total;
            }

            this.ApplySeparation();
            }
        }

        public override void Retreat()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = this.transform.position - playerPosition;
            direction.NormalizeFast();
            direction *= this.movement_speed;
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > this.movement_speed_max)
            {
                velocity.Normalize();
                velocity *= this.movement_speed_max;
            }

            this.Add_Linear_Velocity(velocity);
            this.rotate_to_vector_smooth(direction);

            this.ApplySeparation();
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IProjectile projectile1 && projectile1.FiredByPlayer)
            {
                this.apply_damage(projectile1.Damage);
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

            if (hit.hit_object is Reflect reflect && this.collider != null && this.collider.body != null)
            {
                this.apply_damage(reflect.Damage);
                Box2DX.Common.Vec2 direction = new (this.transform.position.X - hit.hit_position.X, this.transform.position.Y - hit.hit_position.Y);
                if (this.collider != null)
                {
                    this.collider.body.ApplyForce(direction * 100000f, this.collider.body.GetWorldCenter());
                }

                this.set_animation_from_anim_data(this.hitAnim);
            }
        }

        private void ApplySeparation()
        {
            Random random = new ();
            float separationDistance = 80f + ((float)random.NextDouble() * 30f);
            float separationSpeed = 15f + ((float)random.NextDouble() * 10f);
            float maxSeparationForce = 80f;

            Vector2 totalSeparationForce = Vector2.Zero;
            int nearbyCount = 0;

            foreach (var other in this.Controller.characters)
            {
                if (other == this)
                {
                    continue;
                }

                float distance = (other.transform.position - this.transform.position).Length;
                if (distance < separationDistance)
                {
                    Vector2 separationDirection = this.transform.position - other.transform.position;
                    separationDirection.NormalizeFast();

                    float separationForceMagnitude = (float)Math.Exp(-distance / 20f) * maxSeparationForce;
                    Vector2 separationForce = separationDirection * separationForceMagnitude;
                    totalSeparationForce += separationForce;
                    nearbyCount++;
                }
            }

            if (nearbyCount > 0)
            {
                totalSeparationForce /= nearbyCount;

                float jitterAngle = (float)(random.NextDouble() - 0.5f) * 0.5f;
                totalSeparationForce = RotateVector(totalSeparationForce, jitterAngle);

                Box2DX.Common.Vec2 separationVelocity =
                    new Box2DX.Common.Vec2(totalSeparationForce.X, totalSeparationForce.Y) * Game_Time.delta;

                if (separationVelocity.Length() > separationSpeed)
                {
                    separationVelocity.Normalize();
                    separationVelocity *= separationSpeed;
                }

                this.Add_Linear_Velocity(separationVelocity);
            }
        }
    }
}