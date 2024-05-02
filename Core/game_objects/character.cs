using Core.controllers;
using Core.physics;
using Core.util;

namespace Core.game_objects {

    public class character : game_object {

        public float    movement_speed { get; set; } = 100.0f;
        public float    movement_speed_max { get; set; } = 100.0f;
        public int      health { get; set; } = 100;
        public int      health_max { get; set; } = 100;

        public character() {

            //this.set_sprite(new visual.sprite(resource_manager.get_texture("./assets/textures/Spaceship/Spaceship.png")));

        }

        public void set_controller(I_controller controller) {

            this.controller = controller;
            controller.character = this;
        }

        public override void hit(hit_data hit) {
            throw new NotImplementedException();
        }

        // ================================================== private ==================================================

        private I_controller? controller;



    }
}
