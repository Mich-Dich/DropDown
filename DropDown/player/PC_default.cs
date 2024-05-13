using Box2DX.Common;
using Core;
using Core.controllers.player;
using Core.input;
using Core.world;
using OpenTK.Mathematics;

namespace DropDown.player {

    public class PC_Default : Player_Controller {

        public Core.controllers.player.Action move { get; set; }
        public Core.controllers.player.Action look { get; set; }
        public Core.controllers.player.Action sprint { get; set; }

        public PC_Default(Character character)
            : base (character, null) {

            actions.Clear();

            move = new Core.controllers.player.Action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail> {

                    new KeyBindingDetail(Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new KeyBindingDetail(Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new KeyBindingDetail(Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new KeyBindingDetail(Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            AddInputAction(move);


            look = new Core.controllers.player.Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new KeyBindingDetail(Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);


            sprint = new Core.controllers.player.Action(
                "shoot",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new KeyBindingDetail(Key_Code.LeftShift, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(sprint);

        }

        protected override void Update(float deltaTime) {

            float total_speed = character.movementSpeed;
            if((bool)sprint.GetValue()) 
                total_speed += sprint_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0) {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.get_value());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * delta_time);
            }
            
            // camera follows player
            Game.instance.camera.transform.position = character.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);

            Game.instance.camera.Add_Zoom_Offset((float)look.GetValue() / 50);

            // look at mouse
            Vector2 screen_look = Game.instance.Get_Mouse_Relative_Pos();
            float angleRadians = (float)System.Math.Atan2(screen_look.X, screen_look.Y);
            character.transform.rotation = -angleRadians + (float.Pi / 2) + (float.Pi/20);
        }

        private float sprint_speed = 250.0f;

    }
}
