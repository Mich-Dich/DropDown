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
        public float DetectionRange { get; set; } = 400f;
        public SwarmEnemyController Controller { get; set; }
        public float lastFireTime { get; set; }
        public float fireDelay { get; set; } = 1f;
        public animation_data hit_anim;

        public SwarmEnemy() : base() {

            transform.size = new Vector2(40);
            movement_speed = 10;
            movement_speed_max = 20;
            rotation_offset = float.Pi / 2;

            damage = 5;
            ray_number = 15;
            ray_cast_range = 800;
            ray_cast_angle = float.Pi/2;
            auto_detection_range = 100;
            attack_range = 50;

            last_shoot_time = 0f;
            shoot_interval = 0.4f;

            attack_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            walk_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            idle_anim = new animation_data("assets/animation/enemy/enemy.png", 5, 1, true, false, 10, true);
            hit_anim = new animation_data("assets/animation/enemy/enemy-hit.png", 5, 1, true, false, 10, true);
        }

        public bool IsPlayerInRange() {
            
            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - transform.position).Length;
            return distanceToPlayer <= DetectionRange;
        }

        public bool IsPlayerInAttackRange() {
            
            Vector2 playerPosition = Game.Instance.player.transform.position;
            float distanceToPlayer = (playerPosition - transform.position).Length;
            return distanceToPlayer <= StopDistance;
        }

        public bool IsHealthLow() {

            return health <= health_max * 0.2;
        }

        public void Move() {
            
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

        public void Pursue() {
            
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

        public void Attack() {

            if(Game_Time.total - lastFireTime >= fireDelay) {

                Vector2 enemyLocation = this.transform.position;
                Vector2 playerPosition = Game.Instance.player.transform.position;
                Vector2 direction = (playerPosition - enemyLocation).Normalized();
                Game.Instance.get_active_map().Add_Game_Object(new EnemyTestProjectile(enemyLocation, direction));
                lastFireTime = Game_Time.total;
            }

            ApplySeparation();
        }

        public void Retreat() {

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
            if(hit.hit_object is Reflect reflect) {
                this.apply_damage(reflect.Damage);
                Box2DX.Common.Vec2 direction = new(transform.position.X - hit.hit_position.X, transform.position.Y - hit.hit_position.Y);
                collider.body.ApplyForce(direction * 100000f, collider.body.GetWorldCenter());
                set_animation_from_anim_data(hit_anim);
            }
        }
        
        private void ApplySeparation() {

            Random random = new Random();
            float SeparationDistance = 60f + (float)random.NextDouble() * 20f;
            float SeparationSpeed = 10f + (float)random.NextDouble() * 10f;
            foreach (var other in Controller.characters) {
                if (other == this) 
                    continue;
                float distance = (other.transform.position - transform.position).Length;
                if (distance < SeparationDistance) {
                    Vector2 separationDirection = transform.position - other.transform.position;
                    separationDirection.NormalizeFast();
                    separationDirection *= SeparationSpeed;
                    Box2DX.Common.Vec2 separationVelocity = new Box2DX.Common.Vec2(separationDirection.X, separationDirection.Y) * Game_Time.delta;
                    Add_Linear_Velocity(separationVelocity);
                }
            }
        }
    }
}