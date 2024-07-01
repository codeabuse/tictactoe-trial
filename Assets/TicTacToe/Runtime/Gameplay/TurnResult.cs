using System.Linq;
using TicTacToe.Model;
using TicTacToe.Networking;
using UnityEngine;

namespace TicTacToe.Gameplay
{
    public interface ITurnResult
    {
        GameState Evaluate(Board board);
    }
    
    public readonly struct TurnSuccesfull : ITurnResult
    {
        private readonly PlayerMove _move;

        public TurnSuccesfull(PlayerMove move)
        {
            _move = move;
        }
        
        public GameState Evaluate(Board board)
        {
            board.Cells[_move.Position].SetFigure(_move.PlayerId);
            if (board.CheckWinConditions(_move.Position, out var line))
            {
                board.SetWinningLine(line);
                return GameState.GameOver;
            }

            if (!board.Cells.Values.Any(c => c.IsEmpty))
            {
                return GameState.Tie;
            }
            return GameState.WaitForNextTurn;
        }
    }
    
    public readonly struct TurnTimedOut : ITurnResult
    {
        private readonly int _playerId;
        
        public TurnTimedOut(int playerId)
        {
            _playerId = playerId;
        }
        
        public GameState Evaluate(Board board)
        {
            return GameState.TimeOut;
        }
    }

    public readonly struct InvalidMove : ITurnResult
    {
        public GameState Evaluate(Board board)
        {
            return GameState.Terminated;
        }
    }
}