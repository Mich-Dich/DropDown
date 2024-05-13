using Core;
using Core.Controllers.player;
using Core.input;
using Core.world;
using OpenTK.Mathematics;
using System.Runtime.InteropServices.JavaScript;

namespace Projektarbeit.player {

    public class PC_Default : playerController {

        public Core.Controllers.player.Action move { get; set; }
        public Core.Controllers.player.Action sprint { get; set; }

        public PC_Default(Character character)
            : base (character, null) {
            actions.Clear();

            move = new Core.Controllers.player.Action (
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

            sprint = new Core.Controllers.player.Action (
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
                //Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                //character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }
        }

        private float sprint_speed = 5000.0f;

    }
}
