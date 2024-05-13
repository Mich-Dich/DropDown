using Core.world;
using Core.util;

namespace Core.defaults {

    public class Player : Character {

        public Player() {

            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            Set_Sprite(new Core.render.Sprite(Resource_Manager.Get_Texture("assets/textures/player/00.png")));
            
            this.movementSpeed = 5.0f;
        }

    }
}
