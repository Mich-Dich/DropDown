namespace Hell.weapon
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class SniperProjectile : Projectile, IReflectable, IProjectile
    {
        private static readonly Texture Texture = new ("assets/textures/projectiles/beam/beam.png");
        private static readonly Vector2 Size = new (32, 80);

        public bool FiredByPlayer { get; set; } = false;

        private static readonly Collision_Shape Shape = Collision_Shape.Square;

        public bool Reflected { get; private set; } = false;

        public SniperProjectile(Vector2 position, Vector2 direction)
            : base(position, direction, Size, 800f, 20f, Shape)
            {
            Sprite sprite = new (Texture);
            this.Set_Sprite(sprite);
            this.transform.size = Size;
            this.SetSpriteRotation(direction);
        }

        public void set_animation(animation_data animationData)
        {
            if (this.sprite != null)
            {
                Texture textureAtlas = new (animationData.path_to_texture_atlas);
                this.sprite.animation = new Animation(
                    this.sprite,
                    textureAtlas,
                    animationData.num_of_columns,
                    animationData.num_of_rows,
                    animationData.fps,
                    animationData.loop);
            }

            this.sprite.animation.Play();
        }

        public void Reflect(Vector2 position)
        {
            if (!this.Reflected)
            {
                this.Reflected = true;
                Box2DX.Common.Vec2 negativeVelocity = new Box2DX.Common.Vec2(-this.collider.velocity.X, -this.collider.velocity.Y);
                Box2DX.Common.Vec2 force = negativeVelocity * 100000000f;
                Box2DX.Common.Vec2 centerOfMass = this.collider.body.GetWorldCenter();

                this.collider.body.ApplyForce(force, centerOfMass);
                this.rotate_to_vector(this.collider.velocity * -1);
            }
        }

        private void SetSpriteRotation(Vector2 direction)
        {
            float angleRadians = (float)System.Math.Atan2(direction.Y, direction.X);
            this.sprite.transform.rotation = angleRadians + (float)System.Math.PI + ((float)System.Math.PI / 2);
        }
    }
}