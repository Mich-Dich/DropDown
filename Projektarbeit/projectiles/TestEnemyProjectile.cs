using Core.defaults;
using Core.physics;
using Core.render;
using Core.world;
using OpenTK.Mathematics;

namespace Hell.weapon {
    public class EnemyTestProjectile : Projectile {
        private static readonly Texture texture = new Texture("assets/textures/projectiles/beam/beam.png");
        private static readonly Vector2 size = new Vector2(32, 22);
        private static readonly float speed = 4000f;
        private static readonly float damage = 1f;
        private static readonly bool bounce = true;
        private static readonly Collision_Shape shape = Collision_Shape.Square;
        private static readonly animation_data projectileAnimationData = new animation_data("assets/animation/bolt/bolt.png", 1, 4, true, false, 8, true);

        public EnemyTestProjectile(Vector2 position, Vector2 direction) 
            : base(position, direction, speed, damage, bounce, shape) {
            Sprite sprite = new Sprite(texture);
            Set_Sprite(sprite);
            this.transform.size = size;
            set_animation(projectileAnimationData);
            SetSpriteRotation(direction);
        }

        private void SetSpriteRotation(Vector2 direction) {
            float angleRadians = (float)System.Math.Atan2(direction.Y, direction.X);
            sprite.transform.rotation = angleRadians + (float)System.Math.PI;
        }

        public void set_animation(animation_data animationData) {
            if(sprite != null) {
                Texture textureAtlas = new Texture(animationData.path_to_texture_atlas);
                sprite.animation = new Animation(sprite, textureAtlas, animationData.num_of_columns, animationData.num_of_rows, animationData.fps, animationData.loop);
            }

            sprite.animation.Play();
        }
    }
}