
using Core.physics;
using Core.render;
using Core.util;
using Core.defaults;
using Core;
using Box2DX.Common;
using Hell.weapon;
using Core.world;
using OpenTK.Mathematics;

namespace Hell.enemy {

    public class Base_Enemy : Character{

        public Base_Enemy() {

            transform.size = new Vector2(80);
            movement_speed = 500;
            movement_speed_max = 1000;
            movement_force = 5000000;
            rotation_offset = float.Pi;

            Add_Collider(new Collider(Collision_Shape.Circle));

            this.Set_Sprite(new Sprite());
        }

        public void FireBullet() {
            Vector2 enemyLocation = transform.position;
            Vec2 enemyDirectionVec2 = collider.body.GetLinearVelocity();
            enemyDirectionVec2.Normalize();
            Vector2 enemyDirection = new Vector2(enemyDirectionVec2.X, enemyDirectionVec2.Y);
            Game.Instance.get_active_map().Add_Game_Object(new TestProjectile(enemyLocation, enemyDirection));
        }
    }
}
