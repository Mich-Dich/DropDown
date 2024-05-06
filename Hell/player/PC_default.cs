using Core.controllers.player;
using Core.input;
using OpenTK.Mathematics;

namespace Hell {

    public class PC_default : player_controller {

        public action move { get; set; }

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

        }

        protected override void update(float delta_time) {

            character.add_velocity(((Vector2)move.get_value() * character.movement_speed) * delta_time);
        }

    }
}
