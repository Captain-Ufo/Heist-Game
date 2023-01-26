/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System.Diagnostics;

namespace HeistGame
{
    public static class Clock
    {
        private static Stopwatch stopwatch;
        private static long timeAtPreviousFrame;
        private static long deltaTimeMS;

        public static void Initialize()
        {
            stopwatch = new Stopwatch();
        }

        public static void Start()
        {
            stopwatch?.Start();
            timeAtPreviousFrame = stopwatch.ElapsedMilliseconds;
            deltaTimeMS = 0;
        }

        public static void Stop()
        {
            stopwatch?.Stop();
        }

        public static int Tick()
        {
            long currentTime = stopwatch.ElapsedMilliseconds;
            deltaTimeMS = currentTime - timeAtPreviousFrame;
            timeAtPreviousFrame = currentTime;

            return (int)deltaTimeMS;
        }
    }
}
