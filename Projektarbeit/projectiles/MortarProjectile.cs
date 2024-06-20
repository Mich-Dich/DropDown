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
        private readonly Texture shadow;
        private readonly Vector2 size;
        private readonly animation_data projectileAnimationData;
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
            //projectileAnimationData = new animation_data("assets/animation/bolt/bolt.png", 1, 4, true, false, 8, true);

            Sprite sprite = new(texture);
            Set_Sprite(sprite);
            transform.size = size;
            //set_animation(projectileAnimationData);
            SetSpriteRotation(direction);

            this.Direction = direction;

            this.LandingPosition = CalculateLandingPosition(position, direction);
            //Shadow shadow = new Shadow(this.LandingPosition);
            //Game.Instance.get_active_map().Add_Game_Object(shadow);
        }


        public override void Update(float deltaTime)
        {
            // Move the projectile towards the landing position
            if(this.transform.position.Y >=range){
                 this.Direction = Game.Instance.player.transform.position;
            }

            // If the projectile has reached the landing position
            distanceToPlayer = (Game.Instance.player.transform.position - this.transform.position).Length;
            if (this.transform.position == this.LandingPosition)
            {
                // Explode or deal damage
                // ...

                if (distanceToPlayer <= aoeRadius)
                {
                    Core.Game.Instance.player.apply_damage(Damage);
                }

                // Remove the shadow object
                //Game.Instance.get_active_map().Remove_Game_Object(this.shadow);
            }
        }

        private Vector2 CalculateLandingPosition(Vector2 position, Vector2 direction)
        {
            // Calculate where the projectile will land
            // This is just a placeholder, replace it with your actual calculation
            position = Game.Instance.player.transform.position;
            return position + direction * 100;
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
