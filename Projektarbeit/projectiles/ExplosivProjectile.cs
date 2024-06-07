namespace Projektarbeit.projectiles
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class ExplosivProjectile : Projectile, IReflectable, IProjectile
    {
        private readonly Texture texture;
        private readonly animation_data explodeAnimation;
        private readonly animation_data blinkAnimation;
        private readonly float aoeRadius;

        public bool FiredByPlayer { get; set; } = false;

        public bool Reflected { get; private set; } = false;

        public ExplosivProjectile(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(32, 32), 300f, 5f, Collision_Shape.Circle)
        {
            texture = new Texture("assets/textures/projectiles/bomb-1.png");
            explodeAnimation = new animation_data("assets/animation/explosion/explosion-6.png", 1, 8, true, true, 16, false);
            blinkAnimation = new animation_data("assets/animation/projectiles/bomb.png", 1, 6, true, true, 2, false);
            aoeRadius = 130f;

            Sprite sprite = new Sprite(texture);
            Set_Sprite(sprite);
            set_animation(blinkAnimation);
            this.sprite.animation.add_animation_notification(6, () =>
            {
                transform.size = new Vector2(aoeRadius);
                set_animation(explodeAnimation);
                this.sprite.animation.add_animation_notification(5, () =>
                {
                    Vector2 playerPosition = Core.Game.Instance.player.transform.position;
                    Vector2 projectilePosition = transform.position;
                    float distanceToPlayer = (playerPosition - projectilePosition).Length;

                    if (distanceToPlayer <= aoeRadius)
                    {
                        Core.Game.Instance.player.apply_damage(Damage);
                    }
                });
                this.sprite.animation.add_animation_notification(7, () =>
                {
                    Core.Game.Instance.get_active_map().Remove_Game_Object(this);
                });
            });
            SetSpriteRotation(direction);
            Speed = 18f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
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
                collider.body.ApplyForce(new Box2DX.Common.Vec2(-collider.velocity.X, -collider.velocity.Y) * 100000000f, collider.body.GetWorldCenter());
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
