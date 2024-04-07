using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace Core.input {


    // ----------------------- class ----------------------- 
    public class action {

        // ======================================= public =======================================
        public string name { get; set; } = "";
        public bool triger_when_paued { get; set; } = false;
        public action_type action_type { get; set; } = action_type.BOOL;
        public float duration_in_sec { get; set; } = 0;
        public List<key_details> keys { get; set; } = new List<key_details>();

        // payload
        public bool boolean { get; } = false;
        public float vec_1D { get; } = 0;
        public Vector2 vec_2D { get; } = new Vector2();
        public Vector3 vec_3D { get; } = new Vector3();

        public action(String name, bool triger_when_paued, action_type action_type, float duration_in_sec, List<key_details> keys) {

            this.name = name;
            this.triger_when_paued = triger_when_paued;
            this.action_type = action_type;
            this.duration_in_sec = duration_in_sec;
            this.keys = keys;
        }

        public object get_value() {

            switch (action_type) {
                case action_type.BOOL:
                    return boolean;
                case action_type.VEC_1D:
                    return vec_1D;
                case action_type.VEC_2D:
                    return vec_2D;
                case action_type.VEC_3D:
                    return vec_3D;
                default:
                    throw new InvalidOperationException("Unsupported action type.");
            }
        }

        // ======================================= private =======================================
        private bool is_triggered { get; set; } = false;
        private TimeSpan trigger_time {  get; set; } = TimeSpan.Zero;       // time_stamp  of moment this action was last triggered

        // the input system is Event based, meaning wi calculate the target value of this action only once (when an event changes it)
        // but If we want to change the value gradualy (with duration_in_sec) we need to know that the target should be and in waht direction to change the payload calue
        private bool target_boolean { get; set; } = false;
        private float target_vec_1D { get; set; } = 0;
        private Vector2 target_vec_2D { get; set; } = new Vector2();
        private Vector3 target_vec_3D { get; set; } = new Vector3();
    }

    // ----------------------- data ----------------------- 

    public struct key_details {

        public key_code key { get; set; }
        public trigger_flags trigger_flags { get; set; }
        public modefier_flags modefier_flags { get; set; }

        public MouseButton button { get; set; }

        // Static factory method
        public static key_details create(key_code key, trigger_flags triggerFlags, modefier_flags modifierFlags) {
            return new key_details {

                key = key,
                trigger_flags = triggerFlags,
                modefier_flags = modifierFlags
            };
        }
    }

    public enum action_type {

        BOOL = 0,
        VEC_1D = 1,
        VEC_2D = 2,
        VEC_3D = 3,
    }

    [Flags]
    public enum trigger_flags : int {

        none                = 0,
        key_down            = 1 << 0,   // activate input when key is pressed down (can repeat)
        key_up              = 1 << 1,   // activate input when key NOT pressed (can repeat)
        key_hold            = 1 << 2,   // activate input when key down LONGER than [duration_in_sec] in input_action struct (can repeat)
        key_tap             = 1 << 3,   // activate input when key down SHORTER than [duration_in_sec] in input_action struct (can repeat)
        key_move_down       = 1 << 4,   // activate input when starting to press a key (can NOT repeat)
        key_move_up         = 1 << 5,   // activate input when releasing a key (can NOT repeat)

        mouse_positive      = 1 << 10,
        mouse_negative      = 1 << 11,
        mouse_pos_and_neg   = 1 << 12,
    }

    [Flags]
    public enum modefier_flags : int {

        none                = 0,     
        negate              = 1 << 0,
        use_vec_normal      = 1 << 1,
        axis_1              = 1 << 2,
        axis_1_negative     = 1 << 3,
        axis_2              = 1 << 4,
        axis_2_negative     = 1 << 5,
        axis_3              = 1 << 6,
        axis_3_negative     = 1 << 7,
        auto_reset          = 1 << 8,
        auto_resetAll       = 1 << 9, 
    }

}
