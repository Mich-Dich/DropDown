
namespace DropDown.player {

    using Core;
    using Core.physics;
    using Core.util;
    using Core.world;
    using DropDown.maps;
    using OpenTK.Mathematics;

    public class CH_player : Character {

        public float stamina = 70;
        public float stamina_max = 100;

        public uint level = 1;

#if DEBUG
        public uint assigned_AB_point = 20;
#else
        public uint assigned_AB_point = 0;
#endif

        public uint XP_current = 0;
        public uint XP_needed = 10;
        public float health_visual_display_limit = 0.65f;
        private float health_lower_limit_area = 0.3f;

        public CH_player() {

            health = 100;
            health_max = 100;
            auto_heal_amout = 2;

            transform.size = new Vector2(100);
            Set_Sprite(new Sprite(Resource_Manager.Get_Texture("assets/textures/player/00.png")));
            Add_Collider(new Collider(Collision_Shape.Circle)
                .Set_Offset(new Transform(Vector2.Zero, new Vector2(-10))));

            rotation_offset = float.Pi / 2;
            movement_speed = 350.0f;

            death_callback = () => { ((Drop_Down)Game.Instance).set_play_state(DropDown.Game_State.dead); };

        }

        public override void Update(Single deltaTime) {
            base.Update(deltaTime);

            if((health / health_max) <= health_visual_display_limit
                && ((last_blood_stain + 2 * (health / health_max)) <= Game_Time.total)) {

                blood_stuff();
            }
        }

        public override void apply_damage(float damage) {

            float loc_damage = damage;
            if(health <= 0)
                death_callback?.Invoke();

            if((health / health_max) <= health_lower_limit_area)
                loc_damage /= 3;

            base.apply_damage(loc_damage);
            if((health / health_max) <= health_visual_display_limit) {

                blood_stuff();
            }

        }
        
        private void blood_stuff() {

            ((Drop_Down)Game.Instance).ui_HUD.flash_blood_overlay();
            if(typeof(MAP_level) == Game.Instance.get_active_map().GetType()) {

                ((MAP_level)Game.Instance.get_active_map()).add_blood_splater(transform.position);
                last_blood_stain = Game_Time.total;
            }
        }

        public void add_XP(uint amount) {

#if DEBUG
            XP_current += amount * 30;
#else
            XP_current += amount;
#endif
            if(XP_current >= XP_needed) {
                level++;
                assigned_AB_point++;
                XP_current -= XP_needed;
                XP_needed = (uint)(XP_needed * 1.5f);
            }
        }

        public bool has_free_AB_point() { return (assigned_AB_point > 0); }
        public void use_AB_point() { assigned_AB_point--; }

        private float last_blood_stain = 0;

    }
}
