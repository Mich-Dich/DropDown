namespace Core.defaults
{
    using Core.Controllers.player;
    using Core.input;
    using Core.world;
    using OpenTK.Mathematics;

    public class PC_Default : Player_Controller
    {
        public PC_Default(Character character)
            : base(character, null)
            {
            this.actions.Clear();

            this.move = new Action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail>
                {
                    new (Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new (Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new (Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new (Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            this.AddInputAction(this.move);

            this.look = new Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail>
                {
                    new (Key_Code.CursorPositionX, ResetFlags.none, TriggerFlags.mouse_pos_and_neg, KeyModefierFlags.axis_1),
                    new (Key_Code.CursorPositionY, ResetFlags.none, TriggerFlags.mouse_pos_and_neg, KeyModefierFlags.axis_2),
                });
            this.AddInputAction(this.look);
        }

        public Action move { get; set; }

        public Action look { get; set; }

        protected override void Update(float deltaTime)
        {
            // simple movement
            this.character.transform.position += (Vector2)this.move.GetValue() * this.character.movementSpeed;

            // Console.WriteLine($"pos: {player.transform.position}");
            Game.Instance.camera.transform.position = this.character.transform.position;    // TODO: move to game.cs as => player.add_child(camera, attach_mode.lag, 0.2f);

            // transform screen_coord into world_coord
            Vector2 screen_look = (Vector2)this.look.GetValue() - (Game.Instance.camera.transform.size / 2);

            // look at mouse
            float angleRadians = (float)Math.Atan2(screen_look.X, screen_look.Y);
            this.character.transform.rotation = -angleRadians + (float.Pi / 2);
        }
    }
}
