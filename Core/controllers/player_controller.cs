using Core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.controllers {

    public class player_controller : controller{

        public List<input_action> actions { get; set; } = new List<input_action>();


        public player_controller() {

        }

        public player_controller(List<input_action> actions) {

            this.actions = actions;
        }

        public void update(game_time delta_time) {


        }

        // ----------------------------- utility -----------------------------
        public void add_input_action(input_action action) {

            actions.Add(action);
        }

        public bool remove_input_action(input_action action) {

            return actions.Remove(action);
        }

    }
}
