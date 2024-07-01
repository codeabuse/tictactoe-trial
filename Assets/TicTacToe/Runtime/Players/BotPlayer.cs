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

        public BotPlayer(BoardController board, int ownFigureId, int opponentFigureId)
        {
            _board = board;
            _ownFigureId = ownFigureId;
            _opponentFigureId = opponentFigureId;
        }
        
        public async UniTask<ITurnResult> MakeTurn(CancellationToken ct)
        {
            _board.TurnTimer.Reset();
            var timeStart = Time.time;
            _board.TurnTimer.StartCountdown(ct).Forget();
            Analyze(_potentialMoves, ct);
            var position = Decise(_potentialMoves);
            var secondsPassed = Time.time - timeStart;
            await Idle(secondsPassed, ct);
            
            _board.TurnTimer.StopCountodwn();
            var turnRequest = GameClient.CreatePlayerMoveRequest();
            var move = new PlayerMove(_ownFigureId, position);
            return await turnRequest.Send(move).GetResponse();
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
                    var opponentLine = model.GetLineLenght(_opponentFigureId, emptyCell.Position, direction, true);
                    longestOpponentLine = Mathf.Max(
                            longestOpponentLine, 
                            opponentLine.FiguresInLine);
                    danger += opponentLine.FiguresInLine;

                    var ownLine = model.GetLineLenght(_ownFigureId, emptyCell.Position, direction, true);
                    longestOwnLine = Mathf.Max(
                            longestOwnLine,
                            ownLine.FiguresInLine);
                    
                    var freeLine = _board.Model.GetLineLenght(Cell.EmptyCellId, emptyCell.Position, direction,
                            false);
                    if (freeLine.FiguresInLine + ownLine.FiguresInLine >= _rules.WinningLine)
                        quality += ownLine.FiguresInLine;
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

        private async UniTask Idle(float secondsPassed, CancellationToken ct)
        {
            var remainingTime = _rules.TurnTime.TotalSeconds - secondsPassed;
            var random = Random.Range(.25f, .4f);
            await UniTask.Delay((int)(remainingTime * random * 1000), cancellationToken: ct); 
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