using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace TicTacToe.Gameplay
{
    public class Timer
    {
        public event Action<TimeSpan> OnTick;
        public event Action<TimeSpan> OnReset;

        private readonly TimeSpan _duration;
        private readonly TimeSpan _tickDuration;
        private TimeSpan _remainingTime;

        public Timer(TimeSpan duration, uint tickDurationMiliseconds)
        {
            _duration = duration;
            _tickDuration = TimeSpan.FromMilliseconds(tickDurationMiliseconds);
        }
        
        public async UniTask StartCountdown(CancellationToken ct)
        {
            var stopWatch = new Stopwatch();
            var nextTick = _duration;
            while (!ct.IsCancellationRequested && _remainingTime < _duration)
            {
                stopWatch.Restart();
                await UniTask.Delay(nextTick, cancellationToken: ct);
                stopWatch.Stop();
                var lastTick = (int)stopWatch.ElapsedMilliseconds;
                _remainingTime -= TimeSpan.FromMilliseconds(lastTick);
                var durationMilliseconds = _duration.Milliseconds;
                
                if (lastTick > durationMilliseconds)
                {
                    nextTick = TimeSpan.FromMilliseconds(
                            durationMilliseconds - _remainingTime.Milliseconds % durationMilliseconds);
                }
                
                Debug.Log($"Remaining: {_remainingTime.TotalMilliseconds}, Last tick: {lastTick}, Next tick: {nextTick}");
                OnTick?.Invoke(_remainingTime);
            }
        }

        public void Reset()
        {
            _remainingTime = _duration;
            OnReset?.Invoke(_remainingTime);
        }
    }
}