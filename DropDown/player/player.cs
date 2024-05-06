using Core.game_objects;
using Core.physics;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace DropDown.player {

    public class CH_player : character {

        public CH_player() {
            
            transform.size = new Vector2(100);
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/player/00.png")));
            add_collider(new collider(collision_shape.Circle));
                //.set_offset(new transform(Vector2.Zero, new Vector2(-10)))
                //.set_physics_material(new physics_material(0.035f,0.1f)));

            movement_speed = 1500.0f;
        }

        public override void hit(hit_data hit) { }

    }
}
