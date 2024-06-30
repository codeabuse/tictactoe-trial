using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Runtime.Gameplay.GameModes;
using TicTacToe.StaticData;
using UnityEngine;

namespace TicTacToe.Runtime.Gameplay
{
    public enum GameState
    {
        WaitForNextTurn,
        TimeOut,
        GameOver,
    }

    public class GameplayController : MonoBehaviour
    {
        [SerializeField]
        private BoardController _boardController;
        [SerializeField]
        private UIController _uiController;

        private PlayerEntity[] _players;
        private CancellationTokenSource _gameCancellation;
        private RuleSet _rules;

        private void Start()
        {
            _rules = DefaultSettings.DefaultRules;
            
            if (TryGetComponent<IGameMode>(out var gameMode))
            {
                var players = gameMode.CreatePlayers();
                InitializeBoard(players);
                _uiController.Initialize();
            }
            else
            {
                Debug.LogError($"{nameof(IGameMode)} component not found", this);
            }
        }

        private void InitializeBoard(PlayerEntity[] players)
        {
            _boardController.SetupGame(players, _rules).Map(
                    Begin,
                    Debug.LogError);
        }

        private async void Begin()
        {
            _gameCancellation = new();
            var gameState = GameState.WaitForNextTurn;
            while (gameState is GameState.WaitForNextTurn)
            {
                gameState = await GameplayCycle(_gameCancellation.Token);
            }
        }

        private async UniTask<GameState> GameplayCycle(CancellationToken ct)
        {
            var turnResult = await _boardController.NextPlayerTurn(ct);
            GameState state = default;
            
            turnResult.Map(x => state = x, ThrowError);
            return state;
        }

        private static void ThrowError(string message) => throw new GameplayException(message);
    }

    internal class GameplayException : Exception
    {
        public override string Message { get; }

        public GameplayException(string message)
        {
            Message = message;
        }
    }
}