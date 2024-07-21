
namespace DropDown.spells {

    using Core;
    using Core.defaults;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    abstract internal class P_base : Projectile {

        protected bool destroy_after_hit = true;

        public P_base(Vector2 position, Vector2 direction, float speed, float damage, Collision_Shape shape)
            : base(position, direction, new Vector2(50), speed, damage, shape) { }

        public override void Hit(hitData hit) {

            if(hit.hit_object is Character && hit.hit_object != Game.Instance.player) {

                var hit_char = ((Character)hit.hit_object);
                hit_char.apply_damage(Damage);
                hit_char.Add_Linear_Velocity(
                    util.convert_Vector<Box2DX.Common.Vec2>(
                        collider.velocity.Normalized() * projectile_data.knockback.current * 20
                    ));
                
                if (destroy_after_hit)
                    destroy();
            }

        }


    }
}
