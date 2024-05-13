﻿
namespace Projektarbeit.player {

    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class CH_player : Character {
        public CH_player() {
            
            transform.size = new Vector2(100);
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/Angel-1.png")));
            Add_Collider(new Collider(Collision_Shape.Circle)
                .Set_Offset(new Transform(Vector2.Zero, new Vector2(-10)))
                .Set_Physics_Material(new Physics_Material(10.05f, 0.1f)));
            
            movementSpeed = 5000.0f;
        }

        public override void Hit(hitData hit) { }



    }
}
