
namespace DropDown.UI {
    using Core.UI;
    using Core.util;
    using System.Timers;

    internal class UI_death : Menu {

        public Action on_timer_done;
        private readonly Timer timer;
        private float timer_start_time;

        public UI_death(Action timer_done_func) {

            timer = new Timer(5000); // 5 seconds duration
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            on_timer_done = timer_done_func;
        }

        public void Start() { 
            
            timer.Start();
            timer_start_time = Game_Time.total;
        }

        private void OnTimerElapsed(object? source, ElapsedEventArgs e) { on_timer_done?.Invoke(); }


        public override void Render() {
            base.Render();

            // timer_start_time

        }

    }
}
