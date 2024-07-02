using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Model;
using TicTacToe.Networking;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace TicTacToe.Players
{
    public class BotPlayer : IPlayerController
    {
        private readonly BoardController _board;
        private readonly RuleSet _rules;
        private readonly int _ownFigureId;
        private readonly int _opponentFigureId;
        private readonly HashSet<PotentialMove> _potentialMoves = new();
        private float _minIdle;
        private float _maxIdle;

        public BotPlayer(BoardController board, int ownFigureId, int opponentFigureId, float minIdle, float maxIdle)
        {
            _board = board;
            _ownFigureId = ownFigureId;
            _opponentFigureId = opponentFigureId;
            _minIdle = minIdle;
            _maxIdle = maxIdle;
        }
        
        public async UniTask<ITurnResult> MakeTurn(CancellationToken ct)
        {
            _board.TurnTimer.Reset();
            _board.TurnTimer.StartCountdown(ct).Forget();
            
            Analyze(_potentialMoves, ct);
            var position = Decise(_potentialMoves);
            await Idle(ct);
            _board.TurnTimer.StopCountodwn();
            
            var move = new PlayerMove(_ownFigureId, position);
            return new TurnSuccesfull(move);
        }

        private void Analyze(HashSet<PotentialMove> potentialMoves, CancellationToken ct)
        {
            potentialMoves.Clear();
            var model = _board.Model;
            var emptyCells = model.Cells.Values.Where(cell => cell.IsEmpty && model.HasNonEmptyNeighbour(cell));
            
            foreach (var emptyCell in emptyCells)
            {
                int longestOpponentLine = 0;
                int longestOwnLine = 0;
                int danger = 0;
                int quality = 0;
                
                foreach (var direction in BoardTools.LineDirections)
                {
                    var opponentLine = model.GetFiguresOnLine(_opponentFigureId, emptyCell.Position, direction, true);
                    longestOpponentLine = Mathf.Max(
                            longestOpponentLine, 
                            opponentLine.Length);
                    danger += opponentLine.Length;

                    var ownLine = model.GetFiguresOnLine(_ownFigureId, emptyCell.Position, direction, true);
                    longestOwnLine = Mathf.Max(
                            longestOwnLine,
                            ownLine.Length);
                    
                    var freeLine = _board.Model.GetFiguresOnLine(Cell.EmptyCellId, emptyCell.Position, direction,
                            false);
                    if (freeLine.Length + ownLine.Length >= _rules.WinningLine)
                        quality += ownLine.Length;
                }
                        
                potentialMoves.Add(new(emptyCell.Position, longestOpponentLine, longestOwnLine));
            }
        }

        private Vector2Int Decise(HashSet<PotentialMove> potentialMoves)
        {
            if (potentialMoves.Count == 0)
            {
                return MakeRandomTurn();
            }
            if (potentialMoves.Count == 1)
            {
                return potentialMoves.First().Position;
            }
            
            var movesByDanger = ListPool<PotentialMove>.Get();
            movesByDanger.AddRange(potentialMoves);
            var movesByQuality = ListPool<PotentialMove>.Get();
            movesByQuality.AddRange(potentialMoves);
            
            movesByDanger.Sort(PotentialMove.ByDanger);
            movesByQuality.Sort(PotentialMove.ByQuality);
            
            
            Vector2Int move;
            int bestOpponentLine = movesByDanger.Count > 0? movesByDanger[0].LongestOpponentLine : 0;
            int bestOwnLine = movesByQuality.Count > 0 ? movesByQuality[0].LongestOwnLine : 0;

            if (bestOpponentLine < bestOwnLine)
            {
                move = SelectRandomFrom(movesByQuality, m => m.LongestOwnLine == bestOwnLine);
                Debug.Log($"Bot decided to make quality move on {move}, quality = {bestOwnLine}");
            }
            else
            {
                move = SelectRandomFrom(movesByDanger, pm => pm.LongestOpponentLine == bestOpponentLine);
                Debug.Log($"Bot decided to prevent danger move on {move}, danger = {bestOpponentLine}");
            }
            ListPool<PotentialMove>.Release(movesByDanger);
            ListPool<PotentialMove>.Release(movesByQuality);
            
            return move;
        }

        private Vector2Int SelectRandomFrom(List<PotentialMove> moves, Func<PotentialMove, bool> predicate)
        {
            var selected = moves.Where(predicate).ToList();
            if (selected.Count == 0)
                return MakeRandomTurn();
            return selected[Random.Range(0, selected.Count)].Position;
        }

        private Vector2Int MakeRandomTurn()
        {
            Vector2Int turn = default;
            do
            {
                turn = new Vector2Int(Random.Range(0, _rules.BoardDimensions.x), Random.Range(0, _rules.BoardDimensions.y));
            } 
            while (!_board.Model.Cells[turn].IsEmpty);

            return turn;
        }

        private async UniTask Idle(CancellationToken ct)
        {
            var remainingTime = (float)_board.TurnTimer.RemainingTime.TotalSeconds;
            var randomDelay = Random.Range(_minIdle, _maxIdle);
            var delay = Mathf.Min(remainingTime, randomDelay);
            Debug.Log($"Bot idle for {delay}");
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct); 
        }
    }

    internal struct PotentialMove : IEquatable<PotentialMove>
    {
        public readonly Vector2Int Position;
        public readonly int LongestOwnLine;
        public readonly int LongestOpponentLine;
        public int Quality;
        public int Danger;

        public PotentialMove(Vector2Int position, int longestOpponentLine, int longestOwnLine)
        {
            Position = position;
            LongestOwnLine = longestOwnLine;
            LongestOpponentLine = longestOpponentLine;
            Quality = 0;
            Danger = 0;
        }

        public bool Equals(PotentialMove other)
        {
            return Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            return obj is PotentialMove other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
        
        public static int ByDanger(PotentialMove x, PotentialMove y)
        {
            return -x.LongestOpponentLine.CompareTo(y.LongestOpponentLine);
        }
        
        public static int ByQuality(PotentialMove x, PotentialMove y)
        {
            return -x.LongestOwnLine.CompareTo(y.LongestOwnLine);
        }
    }
}