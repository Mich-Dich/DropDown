namespace Hell.enemy {
    using OpenTK.Mathematics;
    using Core.render;
    using Core;
    using Core.util;
    using Hell.weapon;
    using Core.physics;

    public class SwarmEnemy : CH_base_NPC {

        private const float StopDistance = 200f;
        private const float PursueSpeed = 60;

        Random random = new Random();

        public SwarmEnemy() : base() {

            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = float.Pi / 2;

            attack_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walk_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idle_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            hit_anim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public override bool IsPlayerInRange() {
            if (Game.Instance.player == null || Game.Instance.player.IsDead) {
                return false;
            }
            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - transform.position).Length;
            return distanceToPlayer <= DetectionRange;
        }

        public override bool IsPlayerInAttackRange() {
            if (Game.Instance.player == null || Game.Instance.player.IsDead) {
                return false;
            }
            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - transform.position).Length;
            return distanceToPlayer <= StopDistance;
        }

        public override bool IsHealthLow() {

            return health <= health_max * 0.2;
        }

        public override void Move() {
            
            Vector2 direction = new Vector2(0, 1);
            Random random = new Random();
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, 0);
            direction.NormalizeFast();
            direction *= movement_speed;
            movement_force = random.Next((int)movement_speed, (int)movement_speed_max);
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * movement_force * Game_Time.delta;
            if (velocity.Length() > movement_speed_max) {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }
            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);
        }

        public override void Pursue() {
            
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = playerPosition - transform.position;

            if (direction.Length < StopDistance)
                return;

            direction.NormalizeFast();
            direction *= PursueSpeed;
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > movement_speed_max) {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);

            ApplySeparation();
        }

        public override void Attack() {
            if (Game_Time.total - lastFireTime >= fireDelay + (float)random.NextDouble() * (1f - 0.2f) + 0.2f) {
            if(Game_Time.total - lastFireTime >= fireDelay) {

                Vector2 enemyLocation = this.transform.position;
                Vector2 playerPosition = Game.Instance.player.transform.position;
                Vector2 direction = (playerPosition - enemyLocation).Normalized();
                Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(enemyLocation, direction));
                lastFireTime = Game_Time.total;
            }

            ApplySeparation();
            }
        }

        public override void Retreat() {

            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = transform.position - playerPosition;
            direction.NormalizeFast();
            direction *= movement_speed;
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * Game_Time.delta;
            if (velocity.Length() > movement_speed_max) {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }
            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);

            ApplySeparation();
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
            if (hit.hit_object is Reflect reflect && collider != null && collider.body != null) { 
                this.apply_damage(reflect.Damage);
                Box2DX.Common.Vec2 direction = new(transform.position.X - hit.hit_position.X, transform.position.Y - hit.hit_position.Y);
                if(collider != null)
                    collider.body.ApplyForce(direction * 100000f, collider.body.GetWorldCenter());
                set_animation_from_anim_data(hit_anim);
            }
        }
        
        private void ApplySeparation()
        {
            Random random = new Random();
            float separationDistance = 80f + (float)random.NextDouble() * 30f;
            float separationSpeed = 15f + (float)random.NextDouble() * 10f;
            float maxSeparationForce = 80f;

            Vector2 totalSeparationForce = Vector2.Zero;
            int nearbyCount = 0;

            foreach (var other in Controller.characters)
            {
                if (other == this) continue;

                float distance = (other.transform.position - transform.position).Length;
                if (distance < separationDistance)
                {
                    Vector2 separationDirection = transform.position - other.transform.position;
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

                Add_Linear_Velocity(separationVelocity);
            }
        }
    }
}