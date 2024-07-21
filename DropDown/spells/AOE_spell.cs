
namespace DropDown.spells {

    using Core.physics;
    using OpenTK.Mathematics;

    internal class AOE_spell : P_base {

        public AOE_spell(Vector2 position)
            : base(position, new Vector2(0), projectile_data.speed.current, projectile_data.damage.current, Collision_Shape.Circle) {
            
            Lifetime = projectile_data.lifespan.current;
            destroy_after_hit = true;
            Sprite.set_animation("assets/animation/bolt.png", 1, 4, true, false, 30, true);
        }

    }
}
