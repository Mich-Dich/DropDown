using Core;
using Core.game_objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hell {

    public class player : character {

        public player() {

            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            add_sprite(new Core.visual.sprite(game.instance.ResourceManager.GetTexture("assets/textures/Spaceship/Spaceship.png")));
            
        }

    }
}
