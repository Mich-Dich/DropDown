
namespace Projektarbeit.characters.player {

    using Box2DX.Common;
    using Core.Controllers.player;
    using Core.util;
    using Core.world;
    using Hell.weapon;
    using OpenTK.Mathematics;

    internal class PC_main : Player_Controller {

        public Action move { get; set; }
        public Action look { get; set; }
        public Action fire { get; set; }

        private float fireDelay = 0.5f; // Delay in seconds
        private float lastFireTime = 0f;

        public PC_main(Character character)
            : base(character, null) {

            actions.Clear();

            move = new Action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new(Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new(Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new(Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            AddInputAction(move);


            look = new Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);

            fire = new Action(
                "fire",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {
                    new(Key_Code.Space, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(fire);

            Game.Instance.camera.Add_Zoom_Offset(0.2f);
            Game.Instance.camera.zoom_offset = 0.2f;
        }

        protected override void Update(float deltaTime) {

            float total_speed = character.movement_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0) {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }
            
            character.rotate_to_move_dir_smooth();

            if((bool)fire.GetValue() && Game_Time.total - lastFireTime >= fireDelay) {

                Vector2 playerLocation = character.transform.position;
                Vec2 playerDirectionVec2 = character.collider.body.GetLinearVelocity();
                playerDirectionVec2.Normalize();
                Vector2 playerDirection = new Vector2(playerDirectionVec2.X, playerDirectionVec2.Y);
                Game.Instance.get_active_map().Add_Game_Object(new TestProjectile(playerLocation, playerDirection));
                lastFireTime = Game_Time.total;
            }
        }
    }
}
