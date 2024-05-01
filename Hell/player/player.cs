using Core.game_objects;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    public class player : character {

        public player() {

            this.transform.size = new Vector2(50);
            this.transform.rotation = float.Pi;
            add_sprite(new sprite(resource_manager.GetTexture("assets/textures/Spaceship/Spaceship.png")));
        }

    }
}
