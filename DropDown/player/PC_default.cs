using Box2DX.Common;
using Core;
using Core.controllers.player;
using Core.input;
using Core.world;
using OpenTK.Mathematics;

namespace DropDown.player {

    public class PC_default : player_controller {

        public Core.controllers.player.Action move { get; set; }
        public Core.controllers.player.Action look { get; set; }
        public Core.controllers.player.Action sprint { get; set; }

        public PC_default(Character character)
            : base (character, null) {

            actions.Clear();

            move = new Core.controllers.player.Action(
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


            look = new Core.controllers.player.Action(
                "look",
                (uint)action_modefier_flags.none,
                false,
                action_type.VEC_1D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(Key_Code.MouseWheelY, reset_flags.reset_on_key_move_up, trigger_flags.mouse_pos_and_neg),
                });
            add_input_action(look);


            sprint = new Core.controllers.player.Action(
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
            if(move.X != 0 || move.Y != 0) {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.get_value());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * delta_time);
            }
            
            // camera follows player
            Game.instance.camera.transform.position = character.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);

            Game.instance.camera.Add_Zoom_Offset((float)look.get_value() / 50);

            // look at mouse
            Vector2 screen_look = Game.instance.get_mouse_relative_pos();
            float angleRadians = (float)System.Math.Atan2(screen_look.X, screen_look.Y);
            character.transform.rotation = -angleRadians + (float.Pi / 2) + (float.Pi/20);
        }

        private float sprint_speed = 250.0f;

    }
}
