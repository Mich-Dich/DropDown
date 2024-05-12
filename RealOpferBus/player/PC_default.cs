using Core.controllers.player;
using Core.defaults;
using Core.input;
using OpenTK.Mathematics;
using System.Collections.Generic;
using Core;


namespace Hell
{

    public class PC_default : player_controller
    {

        public action move { get; set; }
        public action look { get; set; }
        public action shoot { get; set; }

        public PC_default()
        {

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


            look = new action(
                "look",
                (uint)action_modefier_flags.none,
                false,
                action_type.VEC_1D,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.MouseWheelY, reset_flags.reset_on_key_move_up, trigger_flags.mouse_pos_and_neg),
                });
            add_input_action(look);


            shoot = new action(
                "shoot",
                (uint)action_modefier_flags.none,
                false,
                action_type.BOOL,
                0f,
                new List<key_binding_detail> {

                    new key_binding_detail(key_code.Space, reset_flags.reset_on_key_up, trigger_flags.key_down),
                });
            add_input_action(shoot);
        }

        protected override void update(float delta_time)
        {
            const float acceleration = 1.0f;
            const float friction = 0.06f;
            const float gravity = -100f;

            Vector2 currentVelocity = character.get_velocity();

            Vector2 targetVelocity = new Vector2(move.X, 0) * character.movement_speed;

            Vector2 newVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * delta_time);

            if (move.X == 0)
            {
                newVelocity.X *= (1 - friction);
            }

            newVelocity.Y += gravity * delta_time;

            character.set_velocity(newVelocity);

            if(move.X != 0 || move.Y != 0)
                character.add_velocity(Vector2.NormalizeFast((Vector2)move.get_value()) * character.movement_speed * delta_time);

            if((bool)shoot.get_value())
            {
                ((player)character).Shoot();
            }
        }
    }
}