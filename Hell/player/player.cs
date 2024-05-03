using Core.game_objects;
using Core.util;
using Core.visual;
using Core.physics;
using OpenTK.Mathematics;
using Core.physics.material;

namespace Hell {

    public class player : character {

        public player() {
            this.transform.size = new Vector2(50);
            this.transform.rotation = float.Pi;
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Spaceship/Spaceship.png", true)));
            add_collider(new collider(collision_shape.Circle));
            
            this.movement_speed = 100.0f;
        }

        public override void hit(hit_data hit) {
            //Console.WriteLine("Player collided with an object");
        }

    }
}
