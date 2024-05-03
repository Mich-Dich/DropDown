using Core;
using Core.controllers.player;
using Core.input;
using OpenTK.Mathematics;

namespace Core.defaults {

    public class PC_default : player_controller {

        public action move { get; set; }
        public action look { get; set; }

        public PC_default() {

            this.actions.Clear();

            move = new action(
                "move",
                (uint)action_modefier_flags.auto_reset,
                false,
                action_type.VEC_2D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.W, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_2 | key_modefier_flags.negate),
                    new key_binding_detail(key_code.S, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_2),
                    new key_binding_detail(key_code.D, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_1),
                    new key_binding_detail(key_code.A, reset_flags.reset_on_key_up, trigger_flags.key_down, key_modefier_flags.axis_1 | key_modefier_flags.negate),
                });
            add_input_action(move);


            look = new action(
                "look",
                (uint)action_modefier_flags.none,
                false,
                action_type.VEC_2D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.CursorPositionX, reset_flags.none, trigger_flags.mouse_pos_and_neg, key_modefier_flags.axis_1),
                    new key_binding_detail(key_code.CursorPositionY, reset_flags.none, trigger_flags.mouse_pos_and_neg, key_modefier_flags.axis_2),
                });
            add_input_action(look);
        }

        protected override void update(float delta_time) {
            
            // simple movement
            player.transform.position += ((Vector2)move.get_value() * player.movement_speed);

            //Console.WriteLine($"pos: {player.transform.position}");

            game.instance.camera.transform.position = player.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);

            // transform screen_coord into world_coord
            Vector2 screen_look = (Vector2)look.get_value() - (game.instance.camera.transform.size/2);

            // look at mouse
            float angleRadians = (float)Math.Atan2(screen_look.X, screen_look.Y);
            player.transform.rotation = -angleRadians + float.Pi/2;


        }
    }
}
