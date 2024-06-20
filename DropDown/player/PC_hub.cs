
namespace DropDown.player {

    using Box2DX.Common;
    using Core;
    using Core.Controllers.player;
    using Core.util;
    using Core.world;
    using DropDown.spells;
    using OpenTK.Mathematics;

    public class PC_hub : Player_Controller {

        public Action move { get; set; }
        public Action look { get; set; }
        public Action menu { get; set; }
        public Action interact { get; set; }

        public PC_hub(Character character)
            : base (character, null) {

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

                        
            interact = new Action(
                "shoot",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftMouseButton, ResetFlags.reset_on_key_down, TriggerFlags.key_down),
                });
            AddInputAction(interact);


            menu = new Action(
                "shoot",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftMouseButton, ResetFlags.reset_on_key_down, TriggerFlags.key_down),
                });
            AddInputAction(menu);


            Game.Instance.camera.Add_Zoom_Offset(0.2f);
            Game.Instance.camera.zoom_offset = 0.2f;
        }

        protected override void Update(float deltaTime) {

            // simple movement
            if(move.X != 0 || move.Y != 0) {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * character.movement_speed * deltaTime);
            }

            // camera follows player
            Game.Instance.camera.transform.position = character.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);
            
            // look at mouse
            character.rotate_to_vector(Game.Instance.Get_Mouse_Relative_Pos());
            
            // set zoom
            Game.Instance.camera.Add_Zoom_Offset((float)look.GetValue() / 50);


            if((bool)menu.GetValue()) {

            }
        }

    }
}
