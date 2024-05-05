﻿using Core.game_objects;
using Core.util;

namespace Core.defaults {

    public class player : character {

        public player() {

            this.transform.size = new OpenTK.Mathematics.Vector2(50);
            this.transform.rotation = float.Pi;
            set_sprite(new Core.visual.sprite(resource_manager.get_texture("assets/textures/player/00.png")));
            
            this.movement_speed = 5.0f;
        }

    }
}