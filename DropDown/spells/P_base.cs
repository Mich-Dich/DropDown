
namespace DropDown.spells {
    using Core;
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    internal class P_base : Projectile {

        private readonly animation_data projectileAnimationData;

        public P_base(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(50), 1700, 10, Collision_Shape.Circle) {
            
            Lifetime = 1.0f;
            Sprite.set_animation("assets/animation/bolt.png", 1, 4, true, false, 30, true);
        }

        public override void Hit(hitData hit) {

            Console.WriteLine($"HIT");
            if(hit.hit_object is Character && hit.hit_object != Game.Instance.player) {

                var hit_char = ((Character)hit.hit_object);
                hit_char.apply_damage(Damage);
                hit_char.Add_Linear_Velocity(util.convert_Vector<Box2DX.Common.Vec2>(collider.velocity / 100));
            }

            destroy();
        }

    }
}
