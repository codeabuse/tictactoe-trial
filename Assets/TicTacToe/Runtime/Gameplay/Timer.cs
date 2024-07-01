using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
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

        private CancellationTokenSource _cancellation;

        public Timer(TimeSpan duration, uint tickDurationMiliseconds)
        {
            _duration = duration;
            _tickDuration = TimeSpan.FromMilliseconds(tickDurationMiliseconds);
        }
        
        public UniTask StartCountdown(CancellationToken ct)
        {
            _cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);
            return TimerRoutine(_cancellation.Token);
        }

        private async UniTask TimerRoutine(CancellationToken ct)
        {
            var nextTick = _tickDuration;
            var lastTick = 0;
            while (!ct.IsCancellationRequested && _remainingTime > TimeSpan.FromMilliseconds(0))
            {
                var timeStart = Time.time;
                await UniTask.Delay(_tickDuration, cancellationToken: ct);
                lastTick = (int)((Time.time - timeStart) * 1000);
                _remainingTime -= TimeSpan.FromMilliseconds(lastTick);
                var durationMilliseconds = _duration.TotalMilliseconds;
                
                if (lastTick > durationMilliseconds)
                {
                    nextTick = TimeSpan.FromMilliseconds(durationMilliseconds + durationMilliseconds - lastTick);
                }
                
                //Debug.Log($"Remaining: {_remainingTime.TotalMilliseconds}, Last tick: {lastTick}, Next tick: {nextTick.TotalMilliseconds}");
                OnTick?.Invoke(_remainingTime);
            }
        }

        public void StopCountodwn()
        {
            _cancellation?.Cancel();
        }

        public void Reset()
        {
            _remainingTime = _duration;
            OnReset?.Invoke(_remainingTime);
        }
    }
}