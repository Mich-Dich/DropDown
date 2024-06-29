namespace Projektarbeit.characters.enemy.character
{
    using Core;
    using Core.physics;
    using Core.render;
    using Core.util;
    using OpenTK.Mathematics;
    using Projektarbeit.projectiles;

    public class PullerEnemy : SwarmEnemy
    {
        private const float StopDistance = 450f;
        private const float PursueSpeed = 15;

        public PullerEnemy()
            : base()
        {
            InitializeCharacter();
            InitializeAnimations();
        }

        /* public override void Move()
        {
            Vector2 direction = new Vector2(0, 1);
            Random random = new();
            float offset = (float)(random.NextDouble() - 0.5) * 2;
            direction += new Vector2(offset, 0);
            direction.NormalizeFast();
            direction *= movement_speed;
            movement_force = random.Next((int)movement_speed, (int)movement_speed_max);
            AddVelocityAndRotate(direction);
        } */

        /* public override void Pursue()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = playerPosition - transform.position;

            if (direction.Length < StopDistance)
            {
                return;
            }

            direction.NormalizeFast();
            direction *= PursueSpeed;
            AddVelocityAndRotate(direction);

            ApplySeparation();
        } */

        public override void Attack()
        {
            ApplyForceToPlayer(-3000000);
            ApplySeparation();
        }

        /* public override void Retreat()
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;
            Vector2 direction = transform.position - playerPosition;
            direction.NormalizeFast();
            direction *= movement_speed;
            AddVelocityAndRotate(direction);

            ApplySeparation();
        } */

        private void InitializeCharacter()
        {
            health_max = 100;
            health = health_max;
            fireDelay = 5f;

            transform.size = new Vector2(30);
            movement_speed = 12;
            movement_speed_max = 15;
            rotation_offset = float.Pi / 2;
        }

        private void InitializeAnimations()
        {
            attackAnim = new animation_data("assets/animation/enemy/Puller.png", 5, 1, true, false, 10, true);
            walkAnim = new animation_data("assets/animation/enemy/Puller.png", 5, 1, true, false, 10, true);
            idleAnim = new animation_data("assets/animation/enemy/Puller.png", 5, 1, true, false, 10, true);
            hitAnim = new animation_data("assets/animation/enemy/Puller.png", 5, 1, true, false, 10, true);
        }

        /* private float GetDistanceToPlayer()
        {
            if (Game.Instance.player == null || Game.Instance.player.IsDead)
            {
                return float.MaxValue;
            }

            Vector2 playerPosition = Game.Instance.player.transform.position;
            return (playerPosition - transform.position).Length;
        }

        private void AddVelocityAndRotate(Vector2 direction)
        {
            Box2DX.Common.Vec2 velocity = new Box2DX.Common.Vec2(direction.X, direction.Y) * movement_force * Game_Time.delta;
            if (velocity.Length() > movement_speed_max)
            {
                velocity.Normalize();
                velocity *= movement_speed_max;
            }

            Add_Linear_Velocity(velocity);
            rotate_to_vector_smooth(direction);
        } */

        private void ApplyForceToPlayer(float forceMagnitude)
        {
            if (Game.Instance.player == null || Game.Instance.player.IsDead)
            {
                return;
            }

            if (Game_Time.total - lastFireTime >= fireDelay)
            {
                Vector2 playerPosition = Game.Instance.player.transform.position;
                Vector2 enemyPosition = transform.position;

                // Calculate the direction from the enemy to the player
                Vector2 pullDirection = (playerPosition - enemyPosition).Normalized();

                // Convert OpenTK.Mathematics.Vector2 to Box2DX.Common.Vec2
                Box2DX.Common.Vec2 pullDirectionBox2D = new(pullDirection.X, pullDirection.Y);

                // Apply a force to the player in the direction of the enemy
                Game.Instance.player.add_force(pullDirectionBox2D * forceMagnitude);

                lastFireTime = Game_Time.total;
            }
        }
    }
}
