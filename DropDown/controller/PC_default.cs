using Core.controllers.player;
using Core.input;
using Core.util;
using System;

namespace DropDown
{

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

                    new key_binding_detail(key_code.W, reset_flags.reset_on_key_move_up, trigger_flags.key_down, key_modefier_flags.axis_1),
                    new key_binding_detail(key_code.S, reset_flags.reset_on_key_move_up, trigger_flags.key_down, key_modefier_flags.axis_1 | key_modefier_flags.negate),
                    new key_binding_detail(key_code.D, reset_flags.reset_on_key_move_up, trigger_flags.key_down, key_modefier_flags.axis_2),
                    new key_binding_detail(key_code.A, reset_flags.reset_on_key_move_up, trigger_flags.key_down, key_modefier_flags.axis_2 | key_modefier_flags.negate),
                });
            add_input_action(move);


            look = new action(
                "look",
                (uint)action_modefier_flags.auto_reset,
                false,
                action_type.VEC_2D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.CursorPositionX, reset_flags.reset_on_key_move_up, trigger_flags.mouse_pos_and_neg, key_modefier_flags.none),
                    new key_binding_detail(key_code.CursorPositionY, reset_flags.reset_on_key_move_up, trigger_flags.mouse_pos_and_neg, key_modefier_flags.none),
                });
            add_input_action(look);

        }

        protected override void update(game_time delta_time) {

            Console.WriteLine($"move: {move.get_value()}");

        }
    }
}
