using Core.game_objects;
using Core.util;
using Core.visual;
using Core.physics;
using OpenTK.Mathematics;

namespace Hell {

    public class enemy : character {

        public enemy(Vector2? position = null, Vector2? size = null, Single rotation = 0) {

            this.transform.position = position ?? new Vector2();
            this.transform.size = size?? new Vector2(50);
            this.transform.rotation = rotation;

            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Enemy/Enemy.png", true)));
            add_collider(new collider(collision_shape.Circle));
        }

        public override void hit(hit_data hit) {

            base.hit(hit);
            //Console.WriteLine("Enemy collided with an object");
        }

    }
}