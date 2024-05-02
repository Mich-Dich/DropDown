using Core;
using Core.game_objects;
using Core.physics;
using Core.util;
using OpenTK.Mathematics;

namespace Hell {

    public class base_map : map {

        public base_map() {

            //Texture ateroid_texture = resource_manager.get_texture("assets/textures/Astroids/Astroids.png");
            this.add_game_object(new enemy());
            //this.add_game_object(new game_object(new Vector2(-500, -500), new Vector2(300, 200), 0).set_sprite(ateroid_texture));
            //this.add_game_object(new game_object(new Vector2(100, 500), new Vector2(300, 200), 0).set_sprite(ateroid_texture).set_collider(new collider()));
        }

    }
}
