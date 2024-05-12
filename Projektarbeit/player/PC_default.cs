using Core;
using Core.controllers.player;
using Core.input;
using OpenTK.Mathematics;
using System.Runtime.InteropServices.JavaScript;

namespace Projektarbeit.player {

    public class PC_default : player_controller {

        public action move { get; set; }
        public action sprint { get; set; }

        public PC_default() {

            actions.Clear();

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

            sprint = new action(
                "shoot",
                (uint)action_modefier_flags.none,
                false,
                action_type.BOOL,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.LeftShift, reset_flags.reset_on_key_up, trigger_flags.key_down),
                });
            add_input_action(sprint);

        }

        protected override void update(float delta_time) {

            float total_speed = character.movement_speed;
            if((bool)sprint.get_value()) 
                total_speed += sprint_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0)
                character.add_velocity(Vector2.NormalizeFast((Vector2)move.get_value()) * total_speed * delta_time);
        }

        private float sprint_speed = 5000.0f;

    }
}
