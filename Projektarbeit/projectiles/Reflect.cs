using OpenTK.Mathematics;
using Core.defaults;
using Core.physics;
using Core.render;
using Core.world;

namespace Hell.weapon
{

    public class Reflect : Projectile
    {

        private static readonly animation_data projectileAnimationData = new animation_data("assets/animation/projectiles/swirl.png", 11, 1, true, false, 22, false);

        public Reflect(Vector2 position) : base(position, new Vector2(0, -1), new Vector2(200), 0, 15, false, Collision_Shape.Circle)
        {
            Lifetime = 0.5f;
            this.Sprite.set_animation(projectileAnimationData.path_to_texture_atlas, projectileAnimationData.num_of_rows, projectileAnimationData.num_of_columns, true, true, projectileAnimationData.fps, projectileAnimationData.loop);
        }

        public override void Hit(hitData hit)
        {
            if(hit.hit_object is IReflectable reflectable)
            {
                reflectable.Reflect(hit.hit_position);
                
            }
        }

    }

}