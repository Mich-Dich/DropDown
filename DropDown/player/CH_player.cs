
namespace DropDown.player {
    using Core;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class CH_player : Character {

        public float stamina = 70;
        public float stamina_max = 100;

        public CH_player() {

            health = 100;
            health_max = 100;

            transform.size = new Vector2(100);
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/00.png")));
            Add_Collider(new Collider(Collision_Shape.Circle)
                .Set_Offset(new Transform(Vector2.Zero, new Vector2(-10))));

            rotation_offset = float.Pi/2;
            movement_speed = 350.0f;
        }

        public override void Hit(hitData hit) {

            health -= hit.hit_force;
            ((Drop_Down)Game.Instance).flash_blood_overlay();
        }

    }
}
