using Core.game_objects;
using Core.util;
using OpenTK.Mathematics;
using System.ComponentModel.Design;

namespace Core.controllers.player {

    public abstract class player_controller : controller {

        public character player { get; set; }

        public player_controller() { }

        public player_controller(List<action> actions) {

            this.actions = actions;
        }

        public void update_internal(List<input_event> input_event) {

            for (int x = 0; x < input_event.Count; x++) {
                input_event loc_event = input_event[x];

                for (int y = 0; y < actions.Count; y++) {
                    action loc_action = actions[y];

                    for (int z = 0; z < loc_action.keys_bindings.Count; z++) {
                        key_binding_detail key_binding = loc_action.keys_bindings[z];

                        if (key_binding.key != loc_event.key)       // skip wrong key bindings
                            continue;

                        bool is_key_active = false;

                        // secondary if check to avoid overriding
                        // key_binding can have multiple triggers but only one could be active
                        do {
                            
                            // keyboard flags
                            if((key_binding.trigger_flags & (uint)trigger_flags.key_down) != 0)
                                is_key_active = (loc_event.key_state == key_state.Pressed || loc_event.key_state == key_state.Repeat);

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.key_up) != 0)
                                is_key_active = (loc_event.key_state == key_state.Release);

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.key_hold) != 0)
                                is_key_active = (loc_event.key_state == key_state.Repeat);

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.key_tap) != 0)
                                Console.WriteLine($"Not implemented yet (basicly the same as [trigger_flags.key_move_down] but uses [duration_in_sec])");

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.key_move_down) != 0)
                                is_key_active = (loc_event.key_state == key_state.Pressed);

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.key_move_up) != 0)
                                is_key_active = (loc_event.key_state == key_state.Release);

                            if(is_key_active)
                                break;

                            // mouse flags
                            if((key_binding.trigger_flags & (uint)trigger_flags.mouse_positive) != 0)
                                is_key_active = (loc_event.repeat_amout > 0);

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.mouse_negative) != 0)
                                is_key_active = (loc_event.repeat_amout < 0);

                            if(is_key_active)
                                break;

                            if((key_binding.trigger_flags & (uint)trigger_flags.mouse_pos_and_neg) != 0)
                                is_key_active = true;

                        } while(false);


                        // proccess key_binding modefiers
                        int payload_buffer = is_key_active? 1 : 0;

                        if (key_binding.key == input.key_code.CursorPositionX)
                            payload_buffer = is_key_active ? loc_event.repeat_amout : 0;
                        
                        else if(key_binding.key == input.key_code.CursorPositionY)
                            payload_buffer = is_key_active ? loc_event.repeat_amout : 0;


                        if((key_binding.modefier_flags & (uint)key_modefier_flags.negate) != 0)
                            payload_buffer *= -1;

                        // save value to apropead field
                        switch(loc_action.action_type) {

                            case action_type.BOOL:
                            case action_type.VEC_1D: { loc_action.X = payload_buffer; } break;

                            case action_type.VEC_2D: {

                                if((key_binding.modefier_flags & (uint)key_modefier_flags.axis_2) != 0)
                                    loc_action.Y = payload_buffer;
                                else
                                    loc_action.X = payload_buffer;

                            } break;

                            case action_type.VEC_3D: {

                                if((key_binding.modefier_flags & (uint)key_modefier_flags.axis_3) != 0)
                                    loc_action.Z = payload_buffer;

                                if((key_binding.modefier_flags & (uint)key_modefier_flags.axis_2) != 0)
                                    loc_action.Y = payload_buffer;

                                else
                                    loc_action.X = payload_buffer;
                            } break;

                        }

                    }



                    // proccess action modefiers

                    if((loc_action.modefier_flags & (uint)action_modefier_flags.normalize_vec) != 0) {

                        switch(loc_action.action_type) {

                            case action_type.BOOL: break;
                            case action_type.VEC_1D: break;

                            case action_type.VEC_2D: {

                                Vector2 buffer = (Vector2)loc_action.get_value();
                                if(buffer.Length <= 1)
                                    break;

                                Console.WriteLine($"length: {buffer.Length}");

                                buffer.Normalize();
                                loc_action.X = buffer.X;
                                loc_action.Y = buffer.Y;
                            } break;

                            case action_type.VEC_3D: {

                                Vector3 buffer = (Vector3)loc_action.get_value();
                                if(buffer.Length <= 1)
                                    break;
                            
                                buffer.Normalize();
                                loc_action.X = buffer.X;
                                loc_action.Y = buffer.Y;
                                loc_action.Z = buffer.Z;
                            } break;
                        }
                    }

                }
            }

            
            // call client side code befor resetting payload
            update();

            // resetting payload if needed
            for(int x = 0; x < input_event.Count; x++) {
                input_event loc_event = input_event[x];

                for(int y = 0; y < actions.Count; y++) {
                    action loc_action = actions[y];

                    for(int z = 0; z < loc_action.keys_bindings.Count; z++) {
                        key_binding_detail key_binding = loc_action.keys_bindings[z];

                        if(key_binding.key != loc_event.key)       // skip wrong key bindings
                            continue;

                        // exit early if no reset is needed
                        if(key_binding.reset_flags == (uint)reset_flags.none)
                            continue;

                        bool needs_reset = false;

                        // secondary if check to avoid overriding
                        // key_binding can have multiple resetters but only one could be active
                        do {

                            // keyboard flags
                            if((key_binding.reset_flags & (uint)reset_flags.reset_on_key_down) != 0)
                                needs_reset = (loc_event.key_state == key_state.Pressed || loc_event.key_state == key_state.Repeat);

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_key_up) != 0)
                                needs_reset = (loc_event.key_state == key_state.Release);

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_key_hold) != 0)
                                needs_reset = (loc_event.key_state == key_state.Repeat);

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_key_tap) != 0)
                                Console.WriteLine($"Not implemented yet (basicly the same as [trigger_flags.key_move_down] but uses [duration_in_sec])");

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_key_move_down) != 0)
                                needs_reset = (loc_event.key_state == key_state.Pressed);

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_key_move_up) != 0)
                                needs_reset = (loc_event.key_state == key_state.Release);

                            if(needs_reset)
                                break;

                            // mouse flags
                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_mouse_positive) != 0)
                                needs_reset = (loc_event.repeat_amout > 0);

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_mouse_negative) != 0)
                                needs_reset = (loc_event.repeat_amout < 0);

                            if(needs_reset)
                                break;

                            if((key_binding.trigger_flags & (uint)reset_flags.reset_on_mouse_pos_and_neg) != 0)
                                needs_reset = true;

                        } while(false);

                        // exit early if no reset is needed
                        if(!needs_reset)
                            continue;

                        // save value to apropead field
                        switch(loc_action.action_type) {

                            case action_type.BOOL:
                            case action_type.VEC_1D: { loc_action.X = 0; } break;

                            case action_type.VEC_2D: {

                                if((key_binding.modefier_flags & (uint)key_modefier_flags.axis_2) != 0)
                                    loc_action.Y = 0;
                                else
                                    loc_action.X = 0;

                            }
                            break;

                            case action_type.VEC_3D: {

                                if((key_binding.modefier_flags & (uint)key_modefier_flags.axis_3) != 0)
                                    loc_action.Z = 0;

                                if((key_binding.modefier_flags & (uint)key_modefier_flags.axis_2) != 0)
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

        // ----------------------------- utility -----------------------------
        public void add_input_action(action action) {

            // TODO: (Leonhard) load input data (key_codes) from file
            actions.Add(action);
        }

        public bool remove_input_action(action action) {

            return actions.Remove(action);
        }

        // ============================= protected  ============================= 

        protected List<action> actions { get; set; } = new List<action>();
        protected abstract void update();

    }
}
