using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace Core.controllers {

    // ----------------------- data ----------------------- 

    public struct key_details {

        public Keys key { get; set; }
        public UInt16 trigger_flags { get; set; }
        public UInt16 modefier_flags { get; set;}
    }

    public enum action_type {

        BOOL = 0,
        VEC_1D = 1,
        VEC_2D = 2,
        VEC_3D = 3,
    }

    // ----------------------- class ----------------------- 
    public class input_action {

        // ======================================= public =======================================
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public bool triger_when_paued { get; set; } = false;
        public UInt16 flags { get; set; } = 0;
        public action_type action_Type { get; set; } = action_type.BOOL;
        public bool is_triggered { get; set;}  = false;
        public List<key_details> keys { get; set; } = new List<key_details>();
        public float duration_in_sec { get; set; } = 0;

        // payload
        public bool boolean { get; } = false;
        public float vec_1D { get; } = 0;
        public Vector2 vec_2D { get; } = new Vector2();
        public Vector3 vec_3D { get; } = new Vector3();

        public input_action(String name, String description, Boolean triger_when_paued, UInt16 flags, action_type action_Type, Boolean is_triggered, List<key_details> keys) {

            this.name = name;
            this.description = description;
            this.triger_when_paued = triger_when_paued;
            this.flags = flags;
            this.action_Type = action_Type;
            this.is_triggered = is_triggered;
            this.keys = keys;
        }

        // ======================================= private =======================================
                
        private bool target_boolean { get; set; } = false;
        private float target_vec_1D { get; set; } = 0;
        private Vector2 target_vec_2D { get; set; } = new Vector2();
        private Vector3 target_vec_3D { get; set; } = new Vector3();
        private TimeSpan trigger_time {  get; set; } = TimeSpan.Zero;
    }
}
