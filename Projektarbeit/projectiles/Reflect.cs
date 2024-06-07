namespace Projektarbeit.projectiles
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using OpenTK.Mathematics;

    public class Reflect : Projectile
    {
        private readonly animation_data projectileAnimationData;

        public Reflect(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(200), 0, 20, Collision_Shape.Circle)
        {
            projectileAnimationData = new animation_data("assets/animation/projectiles/swirl.png", 11, 1, true, false, 22, false);

            Lifetime = 0.5f;
            Sprite.set_animation(
                projectileAnimationData.path_to_texture_atlas,
                projectileAnimationData.num_of_rows,
                projectileAnimationData.num_of_columns,
                true,
                true,
                projectileAnimationData.fps,
                projectileAnimationData.loop);
        }

        public override void Hit(hitData hit)
        {
            if (hit.hit_object is IReflectable reflectable)
            {
                reflectable.Reflect(hit.hit_position);
            }
        }
    }
}
