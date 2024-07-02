using System;
using System.Threading;
using Cysharp.Threading.Tasks;

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
        public TimeSpan RemainingTime => _remainingTime;

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
            while (!ct.IsCancellationRequested && _remainingTime > TimeSpan.FromMilliseconds(0))
            {
                await UniTask.Delay(_tickDuration, cancellationToken: ct);
                _remainingTime -= _tickDuration;
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