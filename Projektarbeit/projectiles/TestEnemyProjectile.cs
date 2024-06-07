namespace Projektarbeit.projectiles
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class EnemyTestProjectile : Projectile, IReflectable, IProjectile
    {
        private readonly Texture texture;
        private readonly Vector2 size;
        private readonly animation_data projectileAnimationData;

        public bool FiredByPlayer { get; set; } = false;

        public bool Reflected { get; private set; } = false;

        public EnemyTestProjectile(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(32, 22), 350f, 5f, Collision_Shape.Square)
        {
            texture = new Texture("assets/textures/projectiles/beam/beam.png");
            size = new Vector2(32, 22);
            projectileAnimationData = new animation_data("assets/animation/bolt/bolt.png", 1, 4, true, false, 8, true);

            Sprite sprite = new(texture);
            Set_Sprite(sprite);
            transform.size = size;
            set_animation(projectileAnimationData);
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
            sprite.transform.rotation = angleRadians + (float)Math.PI;
        }
    }
}
