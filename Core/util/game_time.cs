using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.util {

    public class game_time {

        // ======================================= public ======================================= 
        public game_time() {

            total = TimeSpan.Zero;
            elapsed = TimeSpan.Zero;
        }

        public game_time(TimeSpan total_time, TimeSpan elapsed_time) {

            total = total_time;
            elapsed = elapsed_time;
        }

        public TimeSpan total { get; set; }
        public TimeSpan elapsed { get; set; }
    }
}
