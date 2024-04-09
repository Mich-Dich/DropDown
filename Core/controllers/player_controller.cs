using Core.input;
using Core.util;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.controllers {

    public abstract class player_controller : controller {

        public player_controller() { }

        public player_controller(List<input.action> actions) {

            this.actions = actions;
        }

        public void update_internal(game_time delta_time, List<input_event> input_event) {

            //Console.WriteLine($"input_event count: {input_event.Count}");

            // TODO: set target_payload_value based on events

            // TODO: interpulate payload_value based on target_payload_value


            for(int x = 0; x < actions.Count; x++) {   // loop over all actions

                for(int y = 0; y < actions[x].keys.Count; y++) {  // loop over all keys in action

                    //actions[x].keys[y].key;

                }
            }
        }

        // ----------------------------- utility -----------------------------
        public void add_input_action(input.action action) {

            actions.Add(action);
        }

        public bool remove_input_action(input.action action) {

            return actions.Remove(action);
        }

        // ============================= protected  ============================= 

        protected List<input.action> actions { get; set; } = new List<input.action>();
        protected abstract void update(game_time delta_time);

    }
}
