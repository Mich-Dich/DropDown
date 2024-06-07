using Core.world;
using OpenTK.Mathematics;

namespace Core.Controllers.player
{
    public abstract class Player_Controller
    {

        public Character character { get; set; }

        // public character character { get; set; }
        public Player_Controller(Character character, List<Action>? actions = null)
        {
            this.actions = actions ?? new List<Action>();
            this.character = character;
        }

        // ----------------------------- utility -----------------------------
        public void AddInputAction(Action action)
        {
            // TODO: (Leonhard) load input data (key_codes) from file
            this.actions.Add(action);
        }

        public bool RemoveInputAction(Action action)
        {
            return this.actions.Remove(action);
        }

        internal void Update_Internal(float deltaTime, List<InputEvent> inputEvent)
        {
            // TODO: make input event driven && make movement physics-based
            // if(InputEvent.Count == 0)
            //    return;
            for (int x = 0; x < inputEvent.Count; x++)
            {
                InputEvent loc_event = inputEvent[x];

                for (int y = 0; y < this.actions.Count; y++)
                {
                    Action loc_action = this.actions[y];

                    for (int z = 0; z < loc_action.keysBindings.Count; z++)
                    {
                        KeyBindingDetail key_binding = loc_action.keysBindings[z];

                        if (key_binding.key != loc_event.key)
                        {
                            continue;
                        }

                        bool isKeyActive = false;

                        // secondary if check to avoid overriding
                        // key_binding can have multiple triggers but only one could be active
                        do
                        {
                            // keyboard flags
                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.key_down) != 0)
                                isKeyActive = loc_event.KeyState == KeyState.Pressed || loc_event.KeyState == KeyState.Repeat;

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.key_up) != 0)
                                isKeyActive = loc_event.KeyState == KeyState.Release;

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.key_hold) != 0)
                                isKeyActive = loc_event.KeyState == KeyState.Repeat;

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.key_tap) != 0)
                                Console.WriteLine($"Not implemented yet (basicly the same as [TriggerFlags.key_move_down] but uses [durationInSec])");

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.key_move_down) != 0)
                                isKeyActive = loc_event.KeyState == KeyState.Pressed;

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.key_move_up) != 0)
                                isKeyActive = loc_event.KeyState == KeyState.Release;

                            if (isKeyActive)
                                break;

                            // mouse flags
                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.mouse_positive) != 0)
                                isKeyActive = loc_event.repeatAmout > 0;

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.mouse_negative) != 0)
                                isKeyActive = loc_event.repeatAmout < 0;

                            if (isKeyActive)
                                break;

                            if ((key_binding.TriggerFlags & (uint)TriggerFlags.mouse_pos_and_neg) != 0)
                                isKeyActive = true;
                        }
                        while (false);

                        // proccess key_binding modefiers
                        int payloadBuffer = isKeyActive ? 1 : 0;

                        if (key_binding.key == util.Key_Code.CursorPositionX
                            || key_binding.key == util.Key_Code.CursorPositionY
                            || key_binding.key == util.Key_Code.MouseWheelX
                            || key_binding.key == util.Key_Code.MouseWheelY)
                            payloadBuffer = isKeyActive ? loc_event.repeatAmout : 0;

                        if ((key_binding.modefierFlags & (uint)KeyModefierFlags.negate) != 0)
                            payloadBuffer *= -1;

                        // save value to apropead field
                        switch (loc_action.ActionType)
                        {
                            case ActionType.BOOL:
                            case ActionType.VEC_1D:
                                loc_action.X = payloadBuffer;

                                break;

                            case ActionType.VEC_2D:
                                {
                                    if ((key_binding.modefierFlags & (uint)KeyModefierFlags.axis_2) != 0)
                                        loc_action.Y = payloadBuffer;
                                    else
                                        loc_action.X = payloadBuffer;
                                }
                                break;

                            case ActionType.VEC_3D:
                                {
                                    if ((key_binding.modefierFlags & (uint)KeyModefierFlags.axis_3) != 0)
                                        loc_action.Z = payloadBuffer;

                                    if ((key_binding.modefierFlags & (uint)KeyModefierFlags.axis_2) != 0)
                                        loc_action.Y = payloadBuffer;
                                    else
                                        loc_action.X = payloadBuffer;
                                }
                                break;
                        }
                    }

                    // proccess Action modefiers
                    if ((loc_action.modefierFlags & (uint)Action_ModefierFlags.normalize_vec) != 0)
                    {
                        switch (loc_action.ActionType)
                        {
                            case ActionType.BOOL: break;
                            case ActionType.VEC_1D: break;

                            case ActionType.VEC_2D:
                                {
                                    Vector2 buffer = (Vector2)loc_action.GetValue();
                                    if (buffer.Length <= 1)
                                        break;

                                    buffer.Normalize();
                                    loc_action.X = buffer.X;
                                    loc_action.Y = buffer.Y;
                                }
                                break;

                            case ActionType.VEC_3D:
                                {
                                    Vector3 buffer = (Vector3)loc_action.GetValue();
                                    if (buffer.Length <= 1)
                                        break;

                                    buffer.Normalize();
                                    loc_action.X = buffer.X;
                                    loc_action.Y = buffer.Y;
                                    loc_action.Z = buffer.Z;
                                }
                                break;
                        }
                    }
                }
            }

            // call client side code befor resetting payload
            this.Update(deltaTime);

            // resetting payload if needed
            for (int x = 0; x < inputEvent.Count; x++)
            {
                InputEvent loc_event = inputEvent[x];

                for (int y = 0; y < this.actions.Count; y++)
                {
                    Action loc_action = this.actions[y];

                    for (int z = 0; z < loc_action.keysBindings.Count; z++)
                    {
                        KeyBindingDetail key_binding = loc_action.keysBindings[z];

                        if (key_binding.key != loc_event.key)
                            continue;

                        // exit early if no reset is needed
                        if (key_binding.ResetFlags == (uint)ResetFlags.none)
                            continue;

                        bool needs_reset = false;

                        // secondary if check to avoid overriding
                        // key_binding can have multiple resetters but only one could be active
                        do
                        {
                            // keyboard flags
                            if ((key_binding.ResetFlags & (uint)ResetFlags.reset_on_key_down) != 0)
                                needs_reset = loc_event.KeyState == KeyState.Pressed || loc_event.KeyState == KeyState.Repeat;

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_key_up) != 0)
                                needs_reset = loc_event.KeyState == KeyState.Release;

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_key_hold) != 0)
                                needs_reset = loc_event.KeyState == KeyState.Repeat;

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_key_tap) != 0)
                                Console.WriteLine($"Not implemented yet (basicly the same as [TriggerFlags.key_move_down] but uses [durationInSec])");

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_key_move_down) != 0)
                                needs_reset = loc_event.KeyState == KeyState.Pressed;

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_key_move_up) != 0)
                                needs_reset = loc_event.KeyState == KeyState.Release;

                            if (needs_reset)
                                break;

                            // mouse flags
                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_mouse_positive) != 0)
                                needs_reset = loc_event.repeatAmout > 0;

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_mouse_negative) != 0)
                                needs_reset = loc_event.repeatAmout < 0;

                            if (needs_reset)
                                break;

                            if ((key_binding.TriggerFlags & (uint)ResetFlags.reset_on_mouse_pos_and_neg) != 0)
                                needs_reset = true;
                        }
                        while (false);

                        // exit early if no reset is needed
                        if (!needs_reset)
                            continue;

                        // save value to apropead field
                        switch (loc_action.ActionType)
                        {
                            case ActionType.BOOL:
                            case ActionType.VEC_1D:
                                loc_action.X = 0;
                                break;

                            case ActionType.VEC_2D:
                                {
                                    if ((key_binding.modefierFlags & (uint)KeyModefierFlags.axis_2) != 0)
                                        loc_action.Y = 0;
                                    else
                                        loc_action.X = 0;
                                }
                                break;

                            case ActionType.VEC_3D:
                                {
                                    if ((key_binding.modefierFlags & (uint)KeyModefierFlags.axis_3) != 0)
                                        loc_action.Z = 0;

                                    if ((key_binding.modefierFlags & (uint)KeyModefierFlags.axis_2) != 0)
                                        loc_action.Y = 0;
                                    else
                                        loc_action.X = 0;
                                }
                                break;
                        }
                    }
                }
            }
        }

        // ============================= protected  =============================
        protected List<Action> actions { get; set; }

        protected abstract void Update(float deltaTime);
    }
}
