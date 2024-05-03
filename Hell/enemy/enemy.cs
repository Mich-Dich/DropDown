using Core.game_objects;
using Core.util;
using Core.visual;
using Core.physics;
using OpenTK.Mathematics;

namespace Hell {

    public class enemy : game_object {

        public enemy() {
            this.transform.size = new Vector2(50);
            this.transform.rotation = float.Pi;
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Enemy/Enemy.png", true)));
            set_collider(new collider());
        }

        public override void hit(hit_data hit) {

            base.hit(hit);
            //Console.WriteLine("Enemy collided with an object");
        }

    }
}