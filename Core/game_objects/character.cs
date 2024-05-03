using Core.controllers;
using Core.physics;
using Core.util;
using OpenTK.Mathematics;

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
            controller.player = this;
        }

        public void set_velocity(Vector2 new_velocity) {

            if(this.collider != null)
                this.collider.velocity = new_velocity;
        }

        public void add_velocity(Vector2 new_velocity) {

            if(this.collider != null)
                this.collider.velocity += new_velocity;
        }


        public override void hit(hit_data hit) {

            Console.WriteLine($"character [{this}] was hit");
        }

        // ================================================== private ==================================================

        private I_controller? controller;



    }
}
