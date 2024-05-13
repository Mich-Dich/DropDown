
namespace DropDown.player {

    using Core.physics;
    using Core.render;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class CH_player : Character {

        public float stamina = 70;
        public float stamina_max = 100;

        public CH_player() {
            
            transform.size = new Vector2(100);
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/00.png")));
            Add_Collider(new Collider(collision_shape.Circle)
                .set_offset(new Transform(Vector2.Zero, new Vector2(-10)))
                .set_physics_material(new physics_material(10.05f, 0.1f)));
            
            movement_speed = 400.0f;
        }

        public override void Hit(hit_data hit) { }



    }
}
