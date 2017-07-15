using System;

namespace TowerDefenseColab.GamePhases.GameLevels
{
    public class GameLevelActionLimiter
    {
        private readonly GameLevelTime _time;
        private readonly TimeSpan _frequency;
        private TimeSpan _lastAction = TimeSpan.MinValue;
        private TimeSpan _lastCanDo = TimeSpan.MinValue;

        public GameLevelActionLimiter(GameLevelTime time, TimeSpan frequency)
        {
            _time = time;
            _frequency = frequency;
        }

        public bool CanDoStuff()
        {
            TimeSpan now = _time.GetCurrent();
            _lastCanDo = now;
            return _lastAction + _frequency < now;
        }

        public void DoingIt()
        {
            _lastAction = _lastCanDo;
        }

        //public bool AttemptStuff()
        //{
        //    TimeSpan now = _time.GetCurrent();
        //    bool result = _lastAction + _frequency < now;
        //    _lastAction = now;
        //    return result;
        //}
    }
}