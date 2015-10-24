using System.Diagnostics;

namespace OutsideSimulator.D3DCore
{
    /// <summary>
    /// Provide a high precision clock for use in our game
    ///  This is also taken from C++ code from Frank Luna's book, "3D Game Programming with DirectX 11"
    /// </summary>
    public class GameTimer
    {
        private double _secondsPerCount;
        private double _deltaTime;

        private long _baseTime;
        private long _pausedTime;
        private long _stopTime;
        private long _prevTime;
        private long _currTime;

        private bool _stopped;

        public GameTimer()
        {
            _secondsPerCount = 0.0;
            _deltaTime = -1.0;
            _baseTime = 0L;
            _pausedTime = 0L;
            _prevTime = 0L;
            _currTime = 0L;
            _stopped = false;

            var countsPerSec = Stopwatch.Frequency;
            _secondsPerCount = 1.0 / countsPerSec;
        }

        /// <summary>
        /// The total time elapsed in the game
        /// </summary>
        public float TotalTime
        {
            get
            {
                if (_stopped)
                {
                    return (float)(((_stopTime - _pausedTime) - _baseTime) * _secondsPerCount);
                }
                else
                {
                    return (float)(((_currTime - _pausedTime) - _baseTime) * _secondsPerCount);
                }
            }
        }

        public float DeltaTime
        {
            get
            {
                return (float)_deltaTime;
            }
        }

        public void Reset()
        {
            var curTime = Stopwatch.GetTimestamp();
            _baseTime = curTime;
            _prevTime = curTime;
            _stopTime = 0;
            _stopped = false;
        }

        public void Start()
        {
            var startTime = Stopwatch.GetTimestamp();
            if (_stopped)
            {
                _pausedTime += (startTime - _stopTime);
                _prevTime = startTime;
                _stopTime = 0;
                _stopped = false;
            }
        }

        public void Stop()
        {
            if (!_stopped)
            {
                var curTime = Stopwatch.GetTimestamp();
                _stopTime = curTime;
                _stopped = true;
            }
        }

        public void Tick()
        {
            if (_stopped)
            {
                _deltaTime = 0.0;
                return;
            }
            else
            {
                var curTime = Stopwatch.GetTimestamp();
                _currTime = curTime;
                _deltaTime = (_currTime - _prevTime) * _secondsPerCount;
                _prevTime = _currTime;
                if (_deltaTime < 0.0)
                {
                    _deltaTime = 0.0;
                }
            }
        }
    }
}
