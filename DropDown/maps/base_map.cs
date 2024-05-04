using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    public class base_map : map {

        public base_map() {

            this.generate_backgound_tile(50, 30);

            physics_material ateroid_phys_mat = new physics_material(0.0f, 0.0f, 0.1f);
            
            Texture ateroid_texture = resource_manager.get_texture("assets/textures/muzzle_flash.jpg");
            this.add_game_object(
                new game_object(new Vector2(550, -350), new Vector2(300, 100))
                    .set_sprite(ateroid_texture)
                    .set_mobility(mobility.STATIC)
                    .add_collider(new collider(collision_shape.Square)));


            this.add_character(new character());

        }

    }
}
