using Core.game_objects;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    public class player : character {

        public player() {

            this.transform.size = new Vector2(50);
            this.transform.rotation = float.Pi;
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Spaceship/Spaceship.png")));

            this.movement_speed = 800.0f;
        }

    }
}
