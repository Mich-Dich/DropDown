namespace Projektarbeit.projectiles
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class SniperProjectile : Projectile, IReflectable, IProjectile
    {
        private readonly Texture texture;
        private readonly Vector2 size;

        public bool FiredByPlayer { get; set; } = false;

        public bool Reflected { get; private set; } = false;

        public SniperProjectile(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(32, 80), 800f, 20f, Collision_Shape.Square)
        {
            texture = new Texture("assets/textures/projectiles/beam/beam.png");
            size = new Vector2(32, 80);

            Sprite sprite = new(texture);
            Set_Sprite(sprite);
            transform.size = size;
            SetSpriteRotation(direction);
        }

        public void set_animation(animation_data animationData)
        {
            if (sprite != null)
            {
                Texture textureAtlas = new(animationData.path_to_texture_atlas);
                sprite.animation = new Animation(
                    sprite,
                    textureAtlas,
                    animationData.num_of_columns,
                    animationData.num_of_rows,
                    animationData.fps,
                    animationData.loop);
            }

            sprite.animation.Play();
        }

        public void Reflect(Vector2 position)
        {
            if (!Reflected)
            {
                Reflected = true;
                Box2DX.Common.Vec2 negativeVelocity = new Box2DX.Common.Vec2(-collider.velocity.X, -collider.velocity.Y);
                Box2DX.Common.Vec2 force = negativeVelocity * 100000000f;
                Box2DX.Common.Vec2 centerOfMass = collider.body.GetWorldCenter();

                collider.body.ApplyForce(force, centerOfMass);
                rotate_to_vector(collider.velocity * -1);
            }
        }

        private void SetSpriteRotation(Vector2 direction)
        {
            float angleRadians = (float)Math.Atan2(direction.Y, direction.X);
            sprite.transform.rotation = angleRadians + (float)Math.PI + ((float)Math.PI / 2);
        }
    }
}
