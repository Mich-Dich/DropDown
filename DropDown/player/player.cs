using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;

namespace DropDown.player {

    public class CH_player : character {

        public CH_player() {

            transform.size = new Vector2(100);
            set_sprite(new Core.visual.sprite(resource_manager.get_texture("assets/textures/player/00.png")));
            add_collider(new collider(collision_shape.Circle)
                .set_offset(new transform(Vector2.Zero, new Vector2(-50)))
                .set_physics_material(new physics_material(0.035f, 0f, 0.1f)));

            movement_speed = 3500.0f;
        }

        public override void hit(hit_data hit) {


        }
    }
}
