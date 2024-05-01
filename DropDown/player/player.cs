using Core.game_objects;
using Core.util;

namespace DropDown
{

    public class player : character {

        public player() {

            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            add_sprite(new Core.visual.sprite(resource_manager.GetTexture("assets/textures/player/00.png")));
            
            this.movement_speed = 5.0f;
        }

    }
}
