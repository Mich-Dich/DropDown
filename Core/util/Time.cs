using System.Diagnostics;

namespace Core.util {

    public static class Time {

        private static Stopwatch stopwatch = new Stopwatch();

        static Time() {

            stopwatch.Start();
        }

        public static double DeltaTime { get; internal set; }

        public static double TotalTime => stopwatch.Elapsed.TotalSeconds;
    }
}