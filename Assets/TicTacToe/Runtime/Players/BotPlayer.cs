using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Model;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace TicTacToe.Players
{
    public class BotPlayer : IPlayerController
    {
        private readonly Board _board;
        private readonly RuleSet _rules;
        private readonly int _ownFigureId;
        private readonly int _opponentFigureId;

        public BotPlayer(Board board, RuleSet rules, int ownFigureId, int opponentFigureId)
        {
            _board = board;
            _rules = rules;
            _ownFigureId = ownFigureId;
            _opponentFigureId = opponentFigureId;
        }
        
        public async UniTask<Vector2Int> MakeTurn(CancellationToken ct)
        {
            var timeStart = Time.time;
            var potentialMoves = await Analyze(ct);
            var move = await Decise(potentialMoves, ct);
            var secondsPassed = Time.time - timeStart;
            await Idle(secondsPassed, ct);
            return move;
        }

        private UniTask<PotentialMove[]> Analyze(CancellationToken ct)
        {
            var emptyCells = _board.Cells.Values.Where(cell => cell.IsEmpty && _board.HasNonEmptyNeighbour(cell));
            var potentialMoves = ListPool<PotentialMove>.Get();
            
            foreach (var emptyCell in emptyCells)
            {
                var longestOpponentLine = 0;
                var longestOwnLine = 0;
                
                foreach (var direction in BoardTools.Lines)
                {
                    longestOpponentLine = Mathf.Max(
                            longestOpponentLine, 
                            _board.GetLineLenght(_opponentFigureId, emptyCell, direction));
                        
                    longestOwnLine = Mathf.Max(
                            longestOwnLine,
                            _board.GetLineLenght(_ownFigureId, emptyCell, direction));
                }
                        
                potentialMoves.Add(new(emptyCell.Position, longestOpponentLine, longestOwnLine));
            }

            return default;
        }

        private async UniTask<Vector2Int> Decise(PotentialMove[] potentialMoves, CancellationToken ct)
        {
            var movesByDanger = ListPool<PotentialMove>.Get();
            movesByDanger.AddRange(potentialMoves);
            var movesByQuality = ListPool<PotentialMove>.Get();
            movesByQuality.AddRange(potentialMoves);
            movesByDanger.Sort(PotentialMove.ByDanger);
            movesByQuality.Sort(PotentialMove.ByQuality);
            return default;
        }

        private async UniTask Idle(float secondsPassed, CancellationToken ct)
        {
            var remainingTime = _rules.TurnTime.Seconds - secondsPassed;
            var random = Random.Range(.25f, .4f);
            await UniTask.Delay((int)(remainingTime * random * 1000), cancellationToken: ct); 
        }
    }

    internal struct PotentialMove : IEquatable<PotentialMove>
    {
        public readonly Vector2Int Position;
        public int Quality;
        public int Danger;

        public PotentialMove(Vector2Int position, int danger, int quality)
        {
            Position = position;
            Quality = danger;
            Danger = quality;
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
            return x.Danger > y.Danger ? 1 : 0;
        }
        
        public static int ByQuality(PotentialMove x, PotentialMove y)
        {
            return x.Quality > y.Quality ? 1 : 0;
        }
    }
}