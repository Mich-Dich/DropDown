using Core;
using Core.controllers.player;
using Core.input;
using Core.world;
using OpenTK.Mathematics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Channels;

namespace DropDown.player {

    public class PC_default : player_controller {

        public action move { get; set; }
        public action look { get; set; }
        public action sprint { get; set; }

        public PC_default(Character character)
            : base (character, null) {

            actions.Clear();

            move = new action(
                "move",
                (uint)action_modefier_flags.auto_reset,
                false,
                action_type.VEC_2D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(Key_Code.W, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_2 | key_modefier_flags.negate),
                    new key_binding_detail(Key_Code.S, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_2),
                    new key_binding_detail(Key_Code.D, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_1),
                    new key_binding_detail(Key_Code.A, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_1 | key_modefier_flags.negate),
                });
            add_input_action(move);


            look = new action(
                "look",
                (uint)action_modefier_flags.none,
                false,
                action_type.VEC_1D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(Key_Code.MouseWheelY, reset_flags.reset_on_key_move_up, trigger_flags.mouse_pos_and_neg),
                });
            add_input_action(look);


            sprint = new action(
                "shoot",
                (uint)action_modefier_flags.none,
                false,
                action_type.BOOL,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(Key_Code.LeftShift, reset_flags.reset_on_key_up, trigger_flags.key_down),
                });
            add_input_action(sprint);

        }

        protected override void update(float delta_time) {

            float total_speed = character.movement_speed;
            if((bool)sprint.get_value()) 
                total_speed += sprint_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0)
                character.Add_Velocity(Vector2.NormalizeFast((Vector2)move.get_value()) * total_speed * delta_time);

            // camera follows player
            Game.instance.camera.transform.position = character.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);

            Game.instance.camera.Add_Zoom_Offset((float)look.get_value() / 50);

            // look at mouse
            Vector2 screen_look = Game.instance.get_mouse_relative_pos();
            float angleRadians = (float)Math.Atan2(screen_look.X, screen_look.Y);
            character.transform.rotation = -angleRadians + (float.Pi / 2) + (float.Pi/20);
        }

        private float sprint_speed = 5000.0f;

    }
}
