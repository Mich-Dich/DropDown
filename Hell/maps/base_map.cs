using Core;
using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using OpenTK.Mathematics;

namespace Hell {

    public class base_map : map {

        public base_map() {
            string tmxFilePath = "assets/levels/TestMap.tmx";
            string tsxFilePath = "assets/levels/Tileset.tsx";
            string tilesetImageFilePath = "assets/levels/Tileset.png";

            LoadLevel(tmxFilePath, tsxFilePath, tilesetImageFilePath);

            //physics_material asteroid_phys_mat = new physics_material(0.0f, 0.0f, 0.1f);
            //Texture asteroid_texture = resource_manager.get_texture("assets/textures/Astroids/astroid.png");
            //this.add_game_object(
            //    new game_object(new Vector2(500, -300), new Vector2(160, 160)).set_sprite(asteroid_texture).set_mobility(mobility.DYNAMIC).add_collider(
            //    new collider(collision_shape.Circle).set_physics_material(asteroid_phys_mat)));
        }

    }
}
