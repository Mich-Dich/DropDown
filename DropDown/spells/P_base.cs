
namespace DropDown.spells {
    using Core;
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    internal class P_base : Projectile {


        public P_base(Vector2 position, Vector2 direction)
            : base(position, direction, new Vector2(50), projectile_data.speed.current, projectile_data.damage.current, Collision_Shape.Circle) {
            
            Lifetime = projectile_data.lifespan.current;
            Sprite.set_animation("assets/animation/bolt.png", 1, 4, true, false, 30, true);
        }

        public override void Hit(hitData hit) {

            Console.WriteLine($"HIT");
            if(hit.hit_object is Character && hit.hit_object != Game.Instance.player) {

                var hit_char = ((Character)hit.hit_object);
                hit_char.apply_damage(Damage);
                hit_char.Add_Linear_Velocity(
                    util.convert_Vector<Box2DX.Common.Vec2>(
                        collider.velocity.Normalized() * projectile_data.knockback.current * 20
                    ));
            }

            destroy();
        }

    }
}
