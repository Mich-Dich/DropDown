
namespace DropDown.player {

    using Core;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class CH_player : Character {

        public float stamina = 70;
        public float stamina_max = 100;

        public uint level = 1;
        public uint XP_current = 0;
        public uint XP_needed = 10;

        public CH_player() {

            health = 100;
            health_max = 100;
            auto_heal_amout = 4;

            transform.size = new Vector2(100);
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/00.png")));
            Add_Collider(new Collider(Collision_Shape.Circle)
                .Set_Offset(new Transform(Vector2.Zero, new Vector2(-10))));

            rotation_offset = float.Pi/2;
            movement_speed = 350.0f;
        }

        public override void apply_damage(float damage) { 

            health -= damage; 
            ((Drop_Down)Game.Instance).flash_blood_overlay();

            base.apply_damage(damage);
        }

        public void add_XP(uint amount) {

            XP_current += amount;
            if(XP_current >= XP_needed) {
                level++;
                XP_current -= XP_needed;
                XP_needed = (uint)(XP_needed*1.5f);
            }
        }


    }
}
