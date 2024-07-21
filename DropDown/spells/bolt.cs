
namespace DropDown.spells {

    using Core.physics;
    using OpenTK.Mathematics;

    internal class bolt : P_base{

        public bolt(Vector2 position, Vector2 direction) 
            : base(position, direction, projectile_data.speed.current, projectile_data.damage.current, Collision_Shape.Circle) {

            Lifetime = projectile_data.lifespan.current;
            destroy_after_hit = true;
            Sprite.set_animation("assets/animation/bolt.png", 1, 4, true, false, 30, true);
        }

    }
}
