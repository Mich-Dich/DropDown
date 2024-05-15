using Box2DX.Common;
using Core;
using Core.Controllers.player;
using Core.input;
using Core.util;
using Core.world;
using Microsoft.VisualBasic;
using OpenTK.Mathematics;

namespace DropDown.player {

    public class PC_Default : Player_Controller {

        public Core.Controllers.player.Action move { get; set; }
        public Core.Controllers.player.Action look { get; set; }
        public Core.Controllers.player.Action sprint { get; set; }
        public Core.Controllers.player.Action interact { get; set; }

        public PC_Default(Character character)
            : base (character, null) {

            actions.Clear();

            move = new Core.Controllers.player.Action(
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


            look = new Core.Controllers.player.Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);

            sprint = new Core.Controllers.player.Action(
                "shoot",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftShift, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(sprint);

            
            interact = new Core.Controllers.player.Action(
                "shoot",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftMouseButton, ResetFlags.reset_on_key_down, TriggerFlags.key_down),
                });
            AddInputAction(interact);


            Game.Instance.camera.Add_Zoom_Offset(0.2f);
            Game.Instance.camera.zoom_offset = 0.2f;
        }

        protected override void Update(float deltaTime) {

            float total_speed = character.movement_speed;
            if((bool)sprint.GetValue()) 
                total_speed += sprint_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0) {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }
            
            // camera follows player
            Game.Instance.camera.transform.position = character.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);

            Game.Instance.camera.Add_Zoom_Offset((float)look.GetValue() / 50);

            // look at mouse
            character.rotate_to_vector(Game.Instance.Get_Mouse_Relative_Pos());


            if((bool)interact.GetValue()) {

                List<Game_Object> intersected_game_objects = new List<Game_Object>();
                character.perception_check(ref intersected_game_objects, (float.Pi/2),  16, 2, 60, true, 1.5f);
                Console.WriteLine($"hit objects count: {intersected_game_objects.Count}");

                foreach(var obj in intersected_game_objects) {

                    Character buffer = (Character)(obj);
                }

            }

        }

        private readonly float sprint_speed = 350.0f;

    }
}
