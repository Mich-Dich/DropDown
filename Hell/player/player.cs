﻿using Core.game_objects;
using Core.physics;
using Core.util;
using Core.visual;
using OpenTK.Mathematics;

namespace Hell {

    public class player : character {

        public player() {
            this.transform.size = new Vector2(100);
            this.transform.position = new Vector2(480, 6080);
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Spaceship/Spaceship.png", true)));
            add_collider(new collider(collision_shape.Circle)).set_mobility(mobility.DYNAMIC);
            
            this.movement_speed = 5.0f;
        }

        public override void hit(hit_data hit) {
            //Console.WriteLine("Player collided with an object");
        }

    }
}
