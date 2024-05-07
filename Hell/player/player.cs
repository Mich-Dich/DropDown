using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    public class player : character {

        public player() {
            this.transform.size = new Vector2(100);
            this.transform.position = new Vector2(480, 6080);
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Spaceship/Spaceship.png", true)));
            add_collider(
                    new collider(collision_shape.Square)
                    .set_physics_material(new physics_material(0.05f, 0.1f)))
                .set_mobility(mobility.DYNAMIC);
            
            this.movement_speed = 1000.0f;
        }

        public override void hit(hit_data hit) {
            //Console.WriteLine("Player collided with an object");
        }

    }
}
