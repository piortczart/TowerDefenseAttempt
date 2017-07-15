using System;
using System.Diagnostics;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class GameLevelTime
    {
        private readonly Stopwatch _time = new Stopwatch();

        private TimeSpan _offset = TimeSpan.Zero;

        public TimeSpan GetCurrent()
        {
            return _time.Elapsed + _offset;
        }

        public void SkipTime(TimeSpan timeToSkip)
        {
            _offset += timeToSkip;
        }

        public void Start()
        {
            _time.Start();
        }

        public void Stop()
        {
            _time.Stop();
        }

        internal void Reset()
        {
            _time.Reset();
        }
    }
}