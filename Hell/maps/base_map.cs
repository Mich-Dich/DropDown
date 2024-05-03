using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;

namespace Hell {

    public class base_map : map {

        public base_map() {

            this.add_character(new enemy(new Vector2(200, -300)));

            physics_material ateroid_phys_mat = new physics_material(0.0f, 0.0f, 0.1f);

            Texture ateroid_texture = resource_manager.get_texture("assets/textures/Astroids/Astroids.png");
            this.add_game_object(
                new game_object(new Vector2(-500, -500), new Vector2(300, 200), 0).set_sprite(ateroid_texture).add_collider(
                new collider().set_physics_material(ateroid_phys_mat).set_mass(1000.0f)));

            this.add_game_object(
                new game_object(new Vector2(500, 200), new Vector2(300, 200), 0).set_sprite(ateroid_texture).add_collider(
                new collider(collision_shape.Circle).set_physics_material(ateroid_phys_mat).set_mass(100.0f)));
        }

    }
}
