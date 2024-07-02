using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.AsyncTools;
using TicTacToe.Gameplay.GameModes;
using TicTacToe.Networking;
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
        Draw,
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
                _players = gameMode.CreatePlayers();
                InitializeBoard(_players);
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
            var gameStartRequest = GameClient.CreateGameStartRequest().Send(_players);
            var gameStartResponse = await gameStartRequest.GetResponse();
            string popupMessage = default;
            gameStartResponse.Map(() => { },
                    e =>
                    {
                        gameState = GameState.Terminated;
                        Debug.LogError(popupMessage = e.Message);
                    });
            while (gameState is GameState.WaitForNextTurn)
            {
                var isCancelled = false;
                (isCancelled, gameState) = await GameplayCycle(_gameCancellation.Token).SuppressCancellationThrow();
                if (isCancelled)
                    return;
            }
            switch (gameState)
            {
                case GameState.Unknown:
                    break;
                case GameState.WaitForNextTurn:
                    break;
                case GameState.TimeOut:
                    popupMessage = RecordTimeOut(_boardController.GetNextPlayer(), _boardController.GetCurrentPlayer());
                    break;
                    
                case GameState.GameOver:
                    popupMessage = RecordGameOver(_boardController.GetCurrentPlayer(), _boardController.GetNextPlayer());
                    break;
                    
                case GameState.Terminated:
                    popupMessage = game_terminated_msg;
                    break;
                    
                case GameState.Draw:
                    popupMessage = RecordDraw(_players);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await _popup.ShowAndWaitResponse(popupMessage, "New Game", Restart);
        }

        private async UniTask<GameState> GameplayCycle(CancellationToken ct)
        {
            var turnResult = await _boardController.NextPlayerTurn(ct);
            GameState state = default;
            
            turnResult.Map(x => state = x, ThrowError);
            return state;
        }

        private async void Restart()
        {
            await UniTask.DelayFrame(1);
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
            NetworkSimulationLog.Close();
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