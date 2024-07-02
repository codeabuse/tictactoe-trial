using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Model;
using TicTacToe.Networking;

namespace TicTacToe.Gameplay
{
    public interface ITurnResult
    {
        UniTask<GameState> Evaluate(Board board);
    }
    
    public readonly struct TurnSuccesfull : ITurnResult
    {
        private readonly PlayerMove _move;

        public TurnSuccesfull(PlayerMove move)
        {
            _move = move;
        }
        
        public async UniTask<GameState> Evaluate(Board board)
        {
            var request = GameClient.CreatePlayerMoveRequest().Send(_move);
            var response = await request.GetResponse();
            GameState gameState = default;
            var move = _move;
            response.Map(() =>
                    {
                        gameState = CheckBoard(board, move);
                    },
                    e =>
                    {
                        gameState = GameState.Terminated;
                    });

            switch (gameState)
            {
                case GameState.GameOver:
                    var gameOverRequest = GameClient.CreateEndGameRequest().Send(
                            new GameOver((_move.PlayerId + 1) % 2, move.PlayerId));
                    await gameOverRequest.GetResponse(); 
                    break;
                
                case GameState.Draw:
                    var drawRequest = GameClient.CreateEndGameRequest().Send(
                            new Draw());
                    await drawRequest.GetResponse(); 
                    break;
                
                default:
                    return gameState;
            }
            return gameState;
        }

        private static GameState CheckBoard(Board board, PlayerMove move)
        {
            board.Cells[move.Position].SetFigure(move.PlayerId);
            if (board.CheckWinConditions(move.Position, out var line))
            {
                board.SetWinningLine(line);
                return GameState.GameOver;
            }

            if (!board.Cells.Values.Any(c => c.IsEmpty))
            {
                return GameState.Draw;
            }
            return GameState.WaitForNextTurn;
        }

        public override string ToString()
        {
            return _move.ToString();
        }
    }
    
    public readonly struct TurnTimedOut : ITurnResult
    {
        private readonly int _playerId;
        
        public TurnTimedOut(int looserPlayerId)
        {
            _playerId = looserPlayerId;
        }
        
        public async UniTask<GameState> Evaluate(Board board)
        {
            var request = GameClient.CreateEndGameRequest().Send(new GameOver((_playerId + 1) % 2, _playerId));
            var result = await request.GetResponse();
            GameState gameState = default;
            result.Map(() => gameState = GameState.TimeOut,
                    e =>
                    {
                        gameState = GameState.Terminated;
                    });
            return gameState;
        }
    }
}