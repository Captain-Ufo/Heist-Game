using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// Plays some chiptunes using the Console.Beep() method.
    /// </summary>
    class ChiptunePlayer
    {
        CancellationTokenSource cts;

        public ChiptunePlayer()
        {
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Stops whichever tune is currently playing
        /// </summary>
        public void StopTune()
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Plays (asyncronously, so it won't block player inputs and other gameplay) the game over tune
        /// </summary>
        public void PlayGameOverTune()
        {
            Task.Run(() => GameOverTune(cts.Token));

        }

        /// <summary>
        /// Plays (asyncronously, so it won't block player inputs and otehr gameplay) the win fanfare at the end of the game
        /// </summary>
        public void PlayGameWinTune()
        {
            Task.Run(() => GameWonTune(cts.Token));
        }

        /// <summary>
        /// Plays (asyncronously) a single beep
        /// </summary>
        /// <param name="frequency">Frequency of the beep</param>
        /// <param name="duration">Duration of the beep</param>
        public void PlaySFX(int frequency, int duration)
        {
            Task.Run(() => Beep(frequency, duration));
        }

        private void GameWonTune(CancellationToken token)
        {
            Vector2[] tune =
            {
                new Vector2(540, 100),
                new Vector2(590, 120),
                new Vector2(640, 140),
                new Vector2(690, 500),
                new Vector2(640, 500),
                new Vector2(590, 200),
                new Vector2(640, 100),
                new Vector2(690, 130),
                new Vector2(740, 500),
                new Vector2(690, 500),
                new Vector2(640, 200),
                new Vector2(690, 100),
                new Vector2(740, 130),
                new Vector2(790, 500),
                new Vector2(740, 500),
                new Vector2(790, 200),
                new Vector2(840, 100),
                new Vector2(890, 120),
                new Vector2(940, 140),
                new Vector2(990, 1500),
            };

            for (int i = 0; i < tune.Length; i++)
            {
                Beep(tune[i].X, tune[i].Y);

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private void GameOverTune(CancellationToken token)
        {
            Vector2[] tune =
            {
                new Vector2(660, 1000),
                new Vector2(528, 1000),
                new Vector2(594, 1000),
                new Vector2(495, 1000),
                new Vector2(528, 1000),
                new Vector2(440, 1000),
                new Vector2(419, 1000),
                new Vector2(495, 1000),
                new Vector2(660, 1000),
                new Vector2(528, 1000),
                new Vector2(594, 1000),
                new Vector2(495, 1000),
                new Vector2(660, 500),
                new Vector2(528, 500),
                new Vector2(670, 1000),
                new Vector2(638, 2000),
            };

            for (int i = 0; i < tune.Length; i++)
            {
                Beep(tune[i].X, tune[i].Y);

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
