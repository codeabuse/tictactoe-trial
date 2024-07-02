using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Networking;
using TicTacToe.Structures;

namespace TicTacToe.Players
{
    public class HumanPlayer : IPlayerController
    {
        private readonly BoardController _board;
        private readonly string _name;
        private readonly int _figureId;

        private CancellationTokenSource _waitCellClickedCancellation;

        public HumanPlayer(string name, int figureId, BoardController boardController)
        {
            _name = name;
            _figureId = figureId;
            _board = boardController;
        }

        public async UniTask<ITurnResult> MakeTurn(CancellationToken ct)
        {
            _board.TurnTimer.Reset();

            _waitCellClickedCancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var playerMoveTask = _board.WaitUserClicksEmptyCell(_waitCellClickedCancellation.Token);
            var timerTask = _board.TurnTimer.StartCountdown(ct);

            var (turnMade, move) = await UniTask.WhenAny(playerMoveTask, timerTask);
            _waitCellClickedCancellation.Cancel();

            if (turnMade)
            {
                _board.TurnTimer.StopCountodwn();
                return new TurnSuccesfull(move);
            }
            return new TurnTimedOut(_figureId);
        }
    }
}