using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TicTacToe.Model;
using TicTacToe.Networking;
using TicTacToe.Structures;
using UnityEngine;

namespace TicTacToe.Gameplay
{
    public class BoardController : MonoBehaviour
    {
        private const int tick_duration_miliseconds = 1000;
        
        private static readonly string InvalidPlayersCountError = "Invalid players count";
        private static readonly string PlayerWithIndexIsNullError = "Player with index {0} is null";
        
        public event Action<int> ActivePlayerChanged; 

        [SerializeField]
        private GridGenerator _gridGenerator;
        [SerializeField]
        private Sprite[] _figures;
        [SerializeField]
        private float _winLineAnimationDuration = 1.5f;
        [SerializeField]
        private float _afterWinLineDelay = 2f;

        private LineRenderer _lineRenderer;
        
        private Board _boardModel;
        private CellViewController[] _cells;
        private PlayerEntity[] _players;
        private Timer _turnTimer;
        private int _currentPlayerId = -1;
        private RuleSet _rules;

        private Dictionary<Vector2Int, CellViewController> _cellsMap;

        public CellViewController[] Cells => _cells;

        public Timer TurnTimer => _turnTimer;
        public Board Model => _boardModel;

        public RuleSet Rules => _rules;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

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
            _boardModel = new Board(rules);
            _cells = _gridGenerator.Generate(
                    rules.BoardDimensions.x, 
                    rules.BoardDimensions.y);
            _cellsMap = _cells.ToDictionary(c => c.Position);
            
            foreach (var cellViewController in _cells)
            {
                cellViewController.Initialize(_boardModel.Cells[cellViewController.Position], _figures);
            }
            
            return ResultVoid<string>.Success;
        }

        public async UniTask<Result<GameState, string>> NextPlayerTurn(CancellationToken ct)
        {
            TurnTimer.Reset();
            var currentPlayer = NextPlayer();
            ActivePlayerChanged?.Invoke(_currentPlayerId);
            var turnResult = await currentPlayer.WaitForTurn(ct);

            var gameState = turnResult.Evaluate(_boardModel);
            if (gameState is GameState.GameOver)
            {
                await ShowWinningLine(ct);
            }
            return gameState;
        }

        private PlayerEntity NextPlayer()
        {
            _currentPlayerId = ++_currentPlayerId % _players.Length;
            return _players[_currentPlayerId];
        }

        public async UniTask<PlayerMove> WaitUserClicksEmptyCell(CancellationToken ct)
        {
            var waitCellClickedCancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);
            
            var waitEmptyCellClickedTasks = _cells
                   .Where(x => x.Model.IsEmpty)
                   .Select(x => x.WaitCellClicked(waitCellClickedCancellation.Token)
                           .SuppressCancellationThrow());
            
            var (_, (_, result)) = await UniTask.WhenAny(waitEmptyCellClickedTasks);
            var cell = result.Model;
            waitCellClickedCancellation.Cancel();

            return new PlayerMove(_currentPlayerId, cell.Position);
        }

        public PlayerEntity GetCurrentPlayer()
        {
            return _players[_currentPlayerId];
        }

        public PlayerEntity GetNextPlayer()
        {
            return _players[(_currentPlayerId + 1) % 2];
        }

        public void Reset()
        {
            _currentPlayerId = -1;
            _lineRenderer.positionCount = 0;
            _boardModel.Clear();
        }
        
        private static readonly Vector3 zOffset = Vector3.back;

        private async UniTask ShowWinningLine(CancellationToken ct)
        {
            var winningLine = _boardModel.WinningLine;

            var start = _cellsMap[winningLine.Start].transform.position + zOffset;
            var end = _cellsMap[winningLine.End].transform.position + zOffset;
            var direction = (winningLine.Start - winningLine.End);
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
            
            await DOTween.To(GetLineEnd, SetLineEnd, end, _winLineAnimationDuration)
                   .ToUniTask(cancellationToken:ct);

            Vector3 GetLineEnd() => _lineRenderer.GetPosition(1);
            void SetLineEnd(Vector3 position) => _lineRenderer.SetPosition(1, position);
        }

        public void SwapPlayersTurns()
        {
            _currentPlayerId = _currentPlayerId++ % 2;
            var (p1, p2) = (_players[0], _players[1]);
            var (f1, f2) = (p2.FigureId, p1.FigureId);
            p1.ChangeFigureId(f1);
            p2.ChangeFigureId(f2);
        }
    }
}