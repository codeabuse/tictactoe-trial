using System;
using Cysharp.Threading.Tasks;
using TicTacToe.Structures;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TicTacToe.Networking
{
    [Serializable]
    public class NetworkSimulation
    {
        public static readonly TimeSpan DEFAULT_RESPONSE_WAIT_TIME = TimeSpan.FromSeconds(3);
        public const int DEFAULT_RESPONSE_ATTEMPTS = 3;
        public const float DEFAULT_TIMEOUT_PROBABILITY = 0.1f;
        
        [SerializeField]
        private float _responseTimeout = (float)DEFAULT_RESPONSE_WAIT_TIME.TotalSeconds;
        [FormerlySerializedAs("_responseAttempts")]
        [SerializeField]
        private int _maxResponseAttempts = DEFAULT_RESPONSE_ATTEMPTS;
        [SerializeField]
        private float _timeoutProbability = DEFAULT_TIMEOUT_PROBABILITY;

        public TimeSpan ResponseTimeout => TimeSpan.FromSeconds(_responseTimeout);
        public int MaxResponseAttempts => _maxResponseAttempts;
        public float TimeoutProbability => _timeoutProbability;
        
        public async UniTask SimulateDelay()
        {
            var randomDelayMax = ResponseTimeout.TotalSeconds * (1 + TimeoutProbability);
            var delay = (Random.Range(0, (float)randomDelayMax) > ResponseTimeout.TotalSeconds)
                    ? randomDelayMax
                    : 0.1;
            Debug.Log($"Simulated delay: {delay}s");
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }

        public async UniTask<ResultVoid<NetworkError>> TryWaitResponse(Func<UniTask> getRsponseTask)
        {
            for (var i = 0; i < MaxResponseAttempts; i++)
            {
                Debug.Log($"Response attempt {i + 1}...");
                var timeoutTask = UniTask.Delay(ResponseTimeout);
                var finishedTask = await UniTask.WhenAny(timeoutTask, getRsponseTask());
                if (finishedTask is 1)
                {
                    return ResultVoid<NetworkError>.Success;
                }
            }
            return new NetworkError("Server response timed out");
        }

        public async UniTask<Result<TResult, NetworkError>> TryWaitResponse<TResult>(Func<UniTask<TResult>> getResponseTask)
        {
            for (var i = 0; i < MaxResponseAttempts; i++)
            {
                var timeoutTask = UniTask.Delay(ResponseTimeout);
                var responseTask = getResponseTask();
                var finishedTask = await UniTask.WhenAny(timeoutTask, responseTask);
                if (finishedTask is 1)
                {
                    return responseTask.GetAwaiter().GetResult();
                }
            }
            return new NetworkError("Server response timed out");
        }
    }
}