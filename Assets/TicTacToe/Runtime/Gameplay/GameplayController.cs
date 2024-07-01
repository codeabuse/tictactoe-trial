using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay.GameModes;
using TicTacToe.StaticData;
using TicTacToe.UI;
using UnityEngine;

namespace TicTacToe.Gameplay
{
    public enum GameState
    {
        Unknown,
        WaitForNextTurn,
        TimeOut,
        GameOver,
        Tie,
        Terminated
    }

    public class GameplayController : MonoBehaviour
    {
        private const string game_terminated_msg = "Game terminated due to sync error";
        
        [SerializeField]
        private BoardController _boardController;
        [SerializeField]
        private PopupController _popup;
        [SerializeField]
        private UIController _uiController;

        private PlayerEntity[] _players;
        private CancellationTokenSource _gameCancellation;
        private RuleSet _rules;
        private int _sessionLosses;
        private int _sessionWins;

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
            _uiController.ShowPlayers(players);
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
                var isCancelled = false;
                (isCancelled, gameState) = await GameplayCycle(_gameCancellation.Token).SuppressCancellationThrow();
                string msg = default;
                if (isCancelled)
                    return;
                switch (gameState)
                {
                    case GameState.Unknown:
                        break;
                    case GameState.WaitForNextTurn:
                        break;
                    case GameState.TimeOut:
                        msg = RecordTimeOut(_boardController.GetNextPlayer(), _boardController.GetCurrentPlayer());
                        await _popup.ShowAndWaitResponse(msg, "New Game", Restart, _gameCancellation.Token);
                        break;
                    
                    case GameState.GameOver:
                        msg = RecordGameOver(_boardController.GetCurrentPlayer(), _boardController.GetNextPlayer());
                        await _popup.ShowAndWaitResponse(msg, "New Game", Restart, _gameCancellation.Token);
                        
                        break;
                    
                    case GameState.Terminated:
                        await _popup.ShowAndWaitResponse(game_terminated_msg, "New Game", Restart, _gameCancellation.Token);
                        break;
                    case GameState.Tie:
                        msg = RecordDraw(_players);
                        await _popup.ShowAndWaitResponse(msg, "New Game", Restart, _gameCancellation.Token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async UniTask<GameState> GameplayCycle(CancellationToken ct)
        {
            var turnResult = await _boardController.NextPlayerTurn(ct);
            GameState state = default;
            
            turnResult.Map(x => state = x, ThrowError);
            return state;
        }

        private void Restart()
        {
            _gameCancellation.Cancel();
            _boardController.Reset();
            _boardController.SwapPlayersTurns();
            _uiController.HideActivePlayerMark();
            Begin();
        }

        private string RecordGameOver(PlayerEntity winner, PlayerEntity looser)
        {
            winner.IncrementWins();
            looser.IncrementLosses();
            return $"{winner.Name} made a winning move!" + GetPlayerStats();
        }

        private string RecordTimeOut(PlayerEntity winner, PlayerEntity looser)
        {
            winner.IncrementWins();
            looser.IncrementLosses();
            return $"{looser.Name} was thinking too long! {winner.Name} won." + GetPlayerStats();
        }

        private string RecordDraw(PlayerEntity[] players)
        {
            foreach (var player in players)
            {
                player.IncrementDraws();
            }
            return $"Draw!" + GetPlayerStats();
        }

        private string GetPlayerStats()
        {
            var localPlayer = _players[0];
            return $"\nWins: {localPlayer.Wins}\nLosses: {localPlayer.Losses}\nDraws: {localPlayer.Draws}";
        }

        private void OnApplicationQuit()
        {
            _gameCancellation.Cancel();
        }

        private static void ThrowError(string message) => throw new GameplayException(message);
    }

    public class GameplayException : Exception
    {
        public override string Message { get; }

        public GameplayException(string message)
        {
            Message = message;
        }
    }
}