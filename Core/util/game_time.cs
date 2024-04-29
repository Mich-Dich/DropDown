using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.util {

    public static class game_time {

        // ======================================= public ======================================= 
        static game_time() {

            total = TimeSpan.Zero;
            elapsed = TimeSpan.Zero;
        }

        //static game_time(TimeSpan total_time, TimeSpan elapsed_time) {

        //    total = total_time;
        //    elapsed = elapsed_time;
        //}

        public static  TimeSpan total { get; set; }
        public static TimeSpan elapsed { get; set; }
    }
}
