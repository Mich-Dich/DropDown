using Core.game_objects;
using Core.physics;
using Core.util;
using OpenTK.Mathematics;

namespace DropDown
{

    public class player : character {

        public player() {
            
            this.transform.size = new Vector2(50);
            set_sprite(new Core.visual.sprite(resource_manager.get_texture("assets/textures/player/00.png")));
            add_collider(new collider(collision_shape.Circle));

            this.movement_speed = 500.0f;
        }

        public override void hit(hit_data hit) {
            Console.WriteLine("Player collided with an object");
        }
    }
}
