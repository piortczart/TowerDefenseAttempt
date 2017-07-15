using System;

namespace TowerDefenseColab.GameMechanisms
{
    public class Animation
    {
        public int FirstFrameIndex { get; set; }
        public int LastFrameIndex { get; set; }

        public TimeSpan AnimationTime => TimeSpan.FromMilliseconds(_framesCount * _frameTime);

        /// <summary>
        /// frames per second
        /// </summary>
        public float Speed { get; set; }

        public int CurrentFrame { get; set; }

        private readonly double _frameTime;
        private readonly int _framesCount;
        private double _currentTime;

        public Animation(int firstFrameIndex, int lastFrameIndex, float speed)
        {
            FirstFrameIndex = firstFrameIndex;
            LastFrameIndex = lastFrameIndex;
            Speed = speed;

            _framesCount = LastFrameIndex - FirstFrameIndex + 1;
            _frameTime = 1.0 / Speed * 1000;
        }

        public void Update(TimeSpan ts)
        {
            _currentTime += ts.TotalMilliseconds;
            _currentTime = _currentTime%(_framesCount*_frameTime);
            CurrentFrame = (int) (_currentTime/_frameTime);
        }
    }
}
