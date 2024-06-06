namespace Hell.weapon
{
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.world;
    using OpenTK.Mathematics;

    public class Reflect : Projectile
    {
        private static readonly animation_data ProjectileAnimationData = new ("assets/animation/projectiles/swirl.png", 11, 1, true, false, 22, false);

        public Reflect(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(200), 0, 20, Collision_Shape.Circle)
        {
            this.Lifetime = 0.5f;
            this.Sprite.set_animation(
                ProjectileAnimationData.path_to_texture_atlas,
                ProjectileAnimationData.num_of_rows,
                ProjectileAnimationData.num_of_columns,
                true,
                true,
                ProjectileAnimationData.fps,
                ProjectileAnimationData.loop);
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