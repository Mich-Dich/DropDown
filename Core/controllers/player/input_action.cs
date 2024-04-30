using Core.input;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Core.controllers.player {


    // ----------------------- class ----------------------- 
    public class action {

        // ======================================= public =======================================
        public string name              { get; set; } = "";
        public uint modefier_flags      { get; set; } = 0;
        public bool triger_when_paued   { get; set; } = false;
        public action_type action_type  { get; set; } = action_type.BOOL;
        public float duration_in_sec    { get; set; } = 0;
        public List<key_binding_detail> keys_bindings { get; set; } = new List<key_binding_detail>();

        // payload
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Z { get; set; } = 0;

        public action(string name, uint modefier_flags, bool triger_when_paued, action_type action_type, float duration_in_sec, List<key_binding_detail> keys) {

            this.name = name;
            this.modefier_flags = modefier_flags;
            this.triger_when_paued = triger_when_paued;
            this.action_type = action_type;
            this.duration_in_sec = duration_in_sec;
            this.keys_bindings = keys;
        }

        public object get_value() {

            switch(action_type) {
                case action_type.BOOL:
                return (X > 0);
            
                case action_type.VEC_1D:
                return X;
            
                case action_type.VEC_2D:
                return new Vector2(X, Y);
            
                case action_type.VEC_3D:
                return new Vector3(X, Y, Z);

                default:
                throw new InvalidOperationException("Unsupported action type.");
            }
        }

        // ======================================= private =======================================
        private bool is_triggered       { get; set; } = false;
        private TimeSpan trigger_time   { get; set; } = TimeSpan.Zero;       // time_stamp  of moment this action was last triggered

        // the input system is Event based, meaning wi calculate the target value of this action only once (when an event changes it)
        // but If we want to change the value gradualy (with duration_in_sec) we need to know that the target should be and in waht direction to change the payload calue
        private bool target_boolean     { get; set; } = false;
        private float target_vec_1D     { get; set; } = 0;
        private Vector2 target_vec_2D   { get; set; } = new Vector2();
        private Vector3 target_vec_3D   { get; set; } = new Vector3();
    }

    // ----------------------- data ----------------------- 

    public readonly struct input_event {

        public key_code key             { get; }
        public KeyModifiers modifiers   { get; }
        public key_state key_state      { get; }
        public int repeat_amout         { get; }

        public input_event(key_code key, KeyModifiers modifiers, int repeat_amout, key_state key_state) {

            this.key = key;
            this.modifiers = modifiers;
            this.repeat_amout = repeat_amout;
            this.key_state = key_state;
        }
    }

    public struct key_binding_detail {

        public key_code key         { get; set; }
        public uint reset_flags     { get; set; } = 0;
        public uint trigger_flags   { get; set; } = 0;
        public uint modefier_flags  { get; set; } = 0;
        public MouseButton button   { get; set; } = 0;

        public key_binding_detail(key_code key, reset_flags reset_flags, trigger_flags triggerFlags, key_modefier_flags modifierFlags) {

            this.key = key;
            this.reset_flags = (uint)reset_flags;
            this.trigger_flags = (uint)triggerFlags;
            this.modefier_flags = (uint)modifierFlags;
        }
    }

    public enum key_state {

        Pressed             = 0,
        Release             = 1,
        Repeat              = 2,
    }

    public enum action_type {

        BOOL                = 0,
        VEC_1D              = 1,
        VEC_2D              = 2,
        VEC_3D              = 3,
    }

    [Flags]
    public enum trigger_flags : uint {

        none                = 0,
        key_down            = 1 << 0,   // activate when key is pressed down (can repeat)
        key_up              = 1 << 1,   // activate when key NOT pressed (can repeat)
        key_hold            = 1 << 2,   // activate when key down LONGER than [duration_in_sec] in input_action struct (can repeat)
        key_tap             = 1 << 3,   // activate when key down SHORTER than [duration_in_sec] in input_action struct (can repeat)
        key_move_down       = 1 << 4,   // activate when starting to press a key (can NOT repeat)
        key_move_up         = 1 << 5,   // activate when releasing a key (can NOT repeat)

        mouse_positive      = 1 << 6,
        mouse_negative      = 1 << 7,
        mouse_pos_and_neg   = 1 << 8,
    }

    [Flags]
    public enum key_modefier_flags : uint {

        none                = 0,
        negate              = 1 << 0,
        axis_1              = 1 << 1,
        axis_2              = 1 << 2,
        axis_3              = 1 << 3,
        auto_reset          = 1 << 4,
        auto_resetAll       = 1 << 5,
    }

    [Flags]
    public enum reset_flags : uint {

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
    public enum action_modefier_flags : uint {

        none                = 0,
        normalize_vec       = 1 << 0,
        auto_reset          = 1 << 1,
    }
}
