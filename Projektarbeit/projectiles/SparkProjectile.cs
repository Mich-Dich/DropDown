namespace Projektarbeit.projectiles
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class SparkProjectile : Projectile, IReflectable, IProjectile
    {
        private readonly Texture texture;
        private readonly Vector2 size;
        private readonly animation_data sparkAnimation;

        public bool FiredByPlayer { get; set; } = false;

        public bool Reflected { get; private set; } = false;

        public SparkProjectile(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(64, 32), 900f, 15f, Collision_Shape.Square) // Increased speed from 600f to 900f
        {
            texture = new Texture("assets/animation/bolt/spark-sheet.png");
            size = new Vector2(64, 32);
            
            // Configure animation for spark-sheet.png (5 frames of 64x32 each)
            // 320x32 image = 1 row, 5 columns
            sparkAnimation = new animation_data("assets/animation/bolt/spark-sheet.png", 1, 5, true, true, 12, true);

            Sprite sprite = new(texture);
            Set_Sprite(sprite);
            transform.size = size;
            set_animation(sparkAnimation);
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
            // Since the spark sprite is horizontally oriented, we need to flip it 180 degrees
            float angleRadians = (float)Math.Atan2(direction.Y, direction.X);
            sprite.transform.rotation = angleRadians + (float)Math.PI; // Add π to flip 180 degrees
        }
    }
}