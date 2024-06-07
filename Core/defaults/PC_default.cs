namespace Core.defaults
{
    using Core.Controllers.player;
    using Core.physics;
    using Core.util;
    using Core.world;
    using OpenTK.Mathematics;

    public class PC_Default : Player_Controller
    {

        public Action move { get; set; }
        public Action look { get; set; }
        public Action sprint { get; set; }
        public Action interact { get; set; }

        public PC_Default(Character character)
            : base(character, null)
        {

            actions.Clear();

            move = new Action(
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


            look = new Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);


            sprint = new Action(
                "shoot",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftShift, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(sprint);


            interact = new Action(
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

        protected override void Update(float deltaTime)
        {

            float total_speed = character.movement_speed;
            if ((bool)sprint.GetValue())
                total_speed += sprint_speed;

            // simple movement
            if (move.X != 0 || move.Y != 0)
            {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                character.Add_Linear_Velocity(new Box2DX.Common.Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }

            character.rotate_to_vector(Game.Instance.Get_Mouse_Relative_Pos());         // look at mouse
            Game.Instance.camera.transform.position = character.transform.position;     // camera follows player
            Game.Instance.camera.Add_Zoom_Offset((float)look.GetValue() / 50);          // change zoom

            if ((bool)interact.GetValue())
            {

                List<Game_Object> intersected_game_objects = new();
                character.perception_check(ref intersected_game_objects, (float.Pi / 2), 16, 2, 60, true, 1.5f);

                foreach (var obj in intersected_game_objects)
                {

                    Character buffer = (Character)(obj);
                    buffer.Hit(new hitData(20));
                }
            }

        }

        private readonly float sprint_speed = 350.0f;

    }
}
