using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Model;
using TicTacToe.StaticData;
using TicTacToe.Structures;
using UnityEngine;
using Timer = TicTacToe.Gameplay.Timer;

namespace TicTacToe.Runtime.Gameplay
{
    public class BoardController : MonoBehaviour
    {
        private const int tick_duration_miliseconds = 1000;
        
        private static readonly string InvalidPlayersCountError = "Invalid players count";
        private static readonly string PlayerWithIndexIsNullError = "Player with index {0} is null";
        //private static readonly Result<PlayerEntity, string> GameNotFinishedError ="Game is not finished yet";
        
        public event Action<int> ActivePlayerChanged; 

        [SerializeField]
        private GridGenerator _gridGenerator;
        
        private Board _boardModel;
        private CellViewController[] _cells;
        private PlayerEntity[] _players;
        private Timer _turnTimer;
        private int _currentPlayerId;
        private RuleSet _rules;

        public CellViewController[] Cells => _cells;

        public Timer TurnTimer => _turnTimer;
        public Board Model => _boardModel;

        public RuleSet Rules => _rules;

        public ResultVoid<string> SetupGame(PlayerEntity[] players, RuleSet rules)
        {
            _rules = rules;

            if (players.Length != 2)
            {
                return InvalidPlayersCountError;
            }

            for (var i = 0; i < players.Length; i++)
            {
                if (players[i] is null)
                {
                    return string.Format(PlayerWithIndexIsNullError, i);
                }
            }

            _players = players;
            _turnTimer = new Timer(rules.TurnTime, tick_duration_miliseconds);
            _boardModel = new Board(rules.BoardDimensions);
            _cells = _gridGenerator.Generate(
                    rules.BoardDimensions.x, 
                    rules.BoardDimensions.y, 
                    position => _boardModel.Cells[position]);

            return ResultVoid<string>.Success;
        }

        public async UniTask<Result<GameState, string>> NextPlayerTurn(CancellationToken ct)
        {
            TurnTimer.Reset();
            var currentPlayer = NextPlayer();
            ActivePlayerChanged?.Invoke(_currentPlayerId);
            var playerTurnTask = currentPlayer.WaitForTurn(ct);
            var timerTask = TurnTimer.StartCountdown(ct);
            
            var (playerTurnFinished, position) = await UniTask.WhenAny(
                    playerTurnTask, 
                    timerTask);

            return playerTurnFinished ? 
                    _boardModel.CheckWinConditions(position) : 
                    GameState.TimeOut;
        }

        private PlayerEntity NextPlayer()
        {
            _currentPlayerId = _players.Length % ++_currentPlayerId;
            return _players[_currentPlayerId];
        }

        public async UniTask<Vector2Int> WaitUserClicksEmptyCell(CancellationToken ct)
        {
            var waitCellClickedCancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);
            
            var waitEmptyCellClickedTasks = _cells
                   .Where(x => x.Model.IsEmpty)
                   .Select(x => x.WaitCellClicked(waitCellClickedCancellation.Token)
                           .SuppressCancellationThrow());
            
            var (_, (_, result)) = await UniTask.WhenAny(waitEmptyCellClickedTasks);
            waitCellClickedCancellation.Cancel();
            return result.Model.Position;
        }
    }
}