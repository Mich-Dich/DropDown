﻿
using Core.physics;
using Core.render;
using Core.util;
using Core.world;
using OpenTK.Mathematics;

namespace Hell.enemy {

    public class Base_Enemy : Character{

        public Base_Enemy() {

            transform.size = new Vector2(80);
            movement_speed = 500;
            movement_speed_max = 1000;
            movement_force = 5000000;
            rotation_offset = float.Pi;

            Add_Collider(new Collider(Collision_Shape.Circle));

            this.Set_Sprite(new Sprite());

        }







    }
}