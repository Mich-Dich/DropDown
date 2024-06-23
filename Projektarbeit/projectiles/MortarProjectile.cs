namespace Projektarbeit.projectiles
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class MortarProjectile : Projectile, IReflectable, IProjectile
    {
        private readonly Texture texture;
        private readonly Vector2 size;
        private readonly Vector2 LandingPosition;
        private readonly float aoeRadius = 130f;
        private readonly float range = 1000f;
        public Vector2 Direction { get; private set; }
        private float distanceToPlayer = 0f;

        public bool FiredByPlayer { get; set; } = false;

        public bool Reflected { get; private set; } = false;

        public MortarProjectile(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(32, 22), 350f, 5f, Collision_Shape.Square)
        {
            texture = new Texture("assets/textures/projectiles/firearrow.png");
            size = new Vector2(32, 22);

            Sprite sprite = new(texture);
            Set_Sprite(sprite);
            transform.size = size;
            SetSpriteRotation(direction);

            this.Direction = direction;

            this.LandingPosition = CalculateLandingPosition(direction);
        }

        public override void Update(float deltaTime)
        {
            Vector2 playerPosition = Game.Instance.player.transform.position;

            if (this.transform.position.Y >= range)
            {
                this.Direction = playerPosition;
            }

            distanceToPlayer = (playerPosition - this.transform.position).Length;
            if (this.transform.position == this.LandingPosition)
            {
                if (distanceToPlayer <= aoeRadius)
                {
                    Core.Game.Instance.player.apply_damage(Damage);
                }
            }
        }

        private static Vector2 CalculateLandingPosition(Vector2 direction)
        {
            Vector2 position = Game.Instance.player.transform.position;
            return position + direction * 100;
        }

        public void Reflect(Vector2 position)
        {
            if (!Reflected)
            {
                Reflected = true;
                var negativeVelocity = new Box2DX.Common.Vec2 { X = -collider.velocity.X, Y = -collider.velocity.Y };
                Box2DX.Common.Vec2 force = negativeVelocity * 100000000f;
                Box2DX.Common.Vec2 centerOfMass = collider.body.GetWorldCenter();

                collider.body.ApplyForce(force, centerOfMass);
                rotate_to_vector(collider.velocity * -1);
            }
        }

        private void SetSpriteRotation(Vector2 direction)
        {
            float angleRadians = (float)Math.Atan2(direction.Y, direction.X);
            sprite.transform.rotation = angleRadians + (float)Math.PI;
        }
    }
}