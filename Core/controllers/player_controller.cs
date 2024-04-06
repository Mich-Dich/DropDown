using Core.util;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.controllers {

    public class player_controller : controller {

        public List<input.action> actions { get; set; } = new List<input.action>();


        public player_controller() {

        }

        public player_controller(List<input.action> actions) {

            this.actions = actions;
        }

        public void update(game_time delta_time) {

            for (int x = 0; x < actions.Count; x++) {   // loop over all actions

                for (int y = 0; y < actions[x].keys.Count; y++) {  // loop over all keys in action

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

    }
}
