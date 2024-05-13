
namespace Core.controllers.player {

    using Core.input;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    // ----------------------- class ----------------------- 
    public sealed class Action {

        // ======================================= public =======================================
        public string name              { get; set; } = "";
        public uint modefierFlags      { get; set; } = 0;
        public bool trigerWhenPaued   { get; set; } = false;
        public ActionType ActionType  { get; set; } = ActionType.BOOL;
        public float durationInSec    { get; set; } = 0;
        public List<KeyBindingDetail> keysBindings { get; set; } = new List<KeyBindingDetail>();

        // payload
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Z { get; set; } = 0;

        public Action(string name, uint modefierFlags, bool trigerWhenPaued, ActionType ActionType, float durationInSec, List<KeyBindingDetail> keys) {

            this.name = name;
            this.modefierFlags = modefierFlags;
            this.trigerWhenPaued = trigerWhenPaued;
            this.ActionType = ActionType;
            this.durationInSec = durationInSec;
            this.keysBindings = keys;
        }

        public object GetValue() {

            switch(ActionType) {
                case ActionType.BOOL:
                return (X > 0);
            
                case ActionType.VEC_1D:
                return X;
            
                case ActionType.VEC_2D:
                return new Vector2(X, Y);
            
                case ActionType.VEC_3D:
                return new Vector3(X, Y, Z);

                default:
                throw new InvalidOperationException("Unsupported Action type.");
            }
        }

        // ======================================= private =======================================
        private bool isTriggered       { get; set; } = false;
        private TimeSpan triggerTime   { get; set; } = TimeSpan.Zero;       // time_stamp  of moment this Action was last triggered

        // the input system is Event based, meaning wi calculate the target value of this Action only once (when an event changes it)
        // but If we want to change the value gradualy (with durationInSec) we need to know that the target should be and in waht direction to change the payload calue
        private bool targetBoolean     { get; set; } = false;
        private float targetVec1D     { get; set; } = 0;
        private Vector2 targetVec2D   { get; set; } = new Vector2();
        private Vector3 targetVec3D   { get; set; } = new Vector3();
    }

    // ----------------------- data ----------------------- 

    public readonly struct InputEvent {

        public Key_Code key             { get; }
        public KeyModifiers modifiers   { get; }
        public KeyState KeyState      { get; }
        public int repeatAmout         { get; }

        public InputEvent(Key_Code key, KeyModifiers modifiers, int repeatAmout, KeyState KeyState) {

            this.key = key;
            this.modifiers = modifiers;
            this.repeatAmout = repeatAmout;
            this.KeyState = KeyState;
        }
    }

    public struct KeyBindingDetail {

        public Key_Code key         { get; set; }
        public uint ResetFlags     { get; set; } = 0;
        public uint TriggerFlags   { get; set; } = 0;
        public uint modefierFlags  { get; set; } = 0;
        public MouseButton button   { get; set; } = 0;

        public KeyBindingDetail(Key_Code key, ResetFlags ResetFlags, TriggerFlags TriggerFlags, KeyModefierFlags modifierFlags = KeyModefierFlags.axis_1) {

            this.key = key;
            this.ResetFlags = (uint)ResetFlags;
            this.TriggerFlags = (uint)TriggerFlags;
            this.modefierFlags = (uint)modifierFlags;
        }
    }

    public enum KeyState {

        Pressed             = 0,
        Release             = 1,
        Repeat              = 2,
    }

    public enum ActionType {

        BOOL                = 0,
        VEC_1D              = 1,
        VEC_2D              = 2,
        VEC_3D              = 3,
    }

    [Flags]
    public enum TriggerFlags : uint {

        none                = 0,
        key_down            = 1 << 0,   // activate when key is pressed down (can repeat)
        key_up              = 1 << 1,   // activate when key NOT pressed (can repeat)
        key_hold            = 1 << 2,   // activate when key down LONGER than [durationInSec] in input_action struct (can repeat)
        key_tap             = 1 << 3,   // activate when key down SHORTER than [durationInSec] in input_action struct (can repeat)
        key_move_down       = 1 << 4,   // activate when starting to press a key (can NOT repeat)
        key_move_up         = 1 << 5,   // activate when releasing a key (can NOT repeat)

        mouse_positive      = 1 << 6,
        mouse_negative      = 1 << 7,
        mouse_pos_and_neg   = 1 << 8,
    }

    [Flags]
    public enum KeyModefierFlags : uint {

        none                = 0,
        negate              = 1 << 0,
        axis_1              = 1 << 1,
        axis_2              = 1 << 2,
        axis_3              = 1 << 3,
        auto_reset          = 1 << 4,
        auto_resetAll       = 1 << 5,
    }

    [Flags]
    public enum ResetFlags : uint {

        none                        = 0,
        reset_on_key_down           = 1 << 0,  
        reset_on_key_up             = 1 << 1,
        reset_on_key_hold           = 1 << 2,
        reset_on_key_tap            = 1 << 3,
        reset_on_key_move_down      = 1 << 4,
        reset_on_key_move_up        = 1 << 5,

        reset_on_mouse_positive     = 1 << 6,
        reset_on_mouse_negative     = 1 << 7,
        reset_on_mouse_pos_and_neg  = 1 << 8,
    }

    [Flags]
    public enum Action_ModefierFlags : uint {

        none                = 0,
        normalize_vec       = 1 << 0,
        auto_reset          = 1 << 1,
    }
}
