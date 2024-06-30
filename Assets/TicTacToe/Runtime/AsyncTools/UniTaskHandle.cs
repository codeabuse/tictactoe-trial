using System.Threading;
using Cysharp.Threading.Tasks;

namespace TicTacToe.AsyncTools
{
    public struct UniTaskHandle
    {
        private CancellationTokenSource _cts;
        private UniTask _task;

        public UniTaskHandle(UniTask task)
        {
            _cts = new();
            _task = task;
            _task.AttachExternalCancellation(_cts.Token);
        }

        public UniTaskHandle(UniTask task, CancellationToken token)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _task = task;
            _task.AttachExternalCancellation(_cts.Token);
        }

        public UniTask Task
        {
            get => _task;
            set
            {
                CancelAndRefresh();
                _task = value;
                _task.AttachExternalCancellation(_cts.Token);
            }
        }

        public void CancelAndRefresh()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
        }
    }
}