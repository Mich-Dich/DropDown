using Core.physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.game_objects {

    public class character : game_object {

        public float movement_speed { get; set; } = 1.0f;
        public float movement_speed_max { get; set; } = 10.0f;

        public character() {

            //this.sprite = new visual.sprite(game.instance.ResourceManager.GetTexture("./assets/textures/Spaceship/Spaceship.png"));

        }

        public override void hit(hit_data hit) {
            throw new NotImplementedException();
        }
    }
}
