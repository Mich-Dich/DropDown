using Core.controllers.player;
using Core.defaults;
using Core.input;
using OpenTK.Mathematics;
using System.Collections.Generic;
using Core;


namespace Hell
{

    public class PC_Default : playerController
    {

        public action move { get; set; }
        public action look { get; set; }
        public action shoot { get; set; }

        public PC_Default()
        {

            actions.Clear();

            move = new action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail> {

                    new KeyBindingDetail(key_code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new KeyBindingDetail(key_code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new KeyBindingDetail(key_code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new KeyBindingDetail(key_code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            AddInputAction(move);


            look = new action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new KeyBindingDetail(key_code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);


            shoot = new action(
                "shoot",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new KeyBindingDetail(key_code.Space, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(shoot);
        }

        protected override void Update(float deltaTime)
        {
            const float acceleration = 1.0f;
            const float friction = 0.06f;
            const float gravity = -100f;

            Vector2 currentVelocity = character.get_velocity();

            Vector2 targetVelocity = new Vector2(move.X, 0) * character.movementSpeed;

            Vector2 newVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * deltaTime);

            if (move.X == 0)
            {
                newVelocity.X *= (1 - friction);
            }

            newVelocity.Y += gravity * deltaTime;

            character.set_velocity(newVelocity);

            if(move.X != 0 || move.Y != 0)
                character.add_velocity(Vector2.NormalizeFast((Vector2)move.GetValue()) * character.movementSpeed * deltaTime);

            if((bool)shoot.GetValue())
            {
                ((player)character).Shoot();
            }
        }
    }
}