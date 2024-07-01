using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Networking;
using TicTacToe.UI;
using TicTacToe.StaticData;

namespace TicTacToe.Players
{
    public class HumanPlayer : IPlayerController
    {
        private readonly BoardController _board;
        private readonly PopupController _popupController;
        private readonly string _name;
        private readonly int _figureId;

        private CancellationTokenSource _waitCellClickedCancellation;

        public HumanPlayer(string name, int figureId, BoardController boardController, PopupController popupController)
        {
            _name = name;
            _figureId = figureId;
            _popupController = popupController;
            _board = boardController;
        }

        public async UniTask<ITurnResult> MakeTurn(CancellationToken ct)
        {
            _board.TurnTimer.Reset();
            await _popupController.ShowAndWaitResponse(
                    GameText.GetPlayerReadyText(_name), 
                    GameText.PlayerReadyButtonText, ct);

            _waitCellClickedCancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var playerMoveTask = _board.WaitUserClicksEmptyCell(_waitCellClickedCancellation.Token);
            var timerTask = _board.TurnTimer.StartCountdown(ct);

            var (turnMade, move) = await UniTask.WhenAny(playerMoveTask, timerTask);

            if (turnMade)
            {
                _waitCellClickedCancellation.Cancel();
                _board.TurnTimer.StopCountodwn();
                var turnRequest = GameClient.CreatePlayerMoveRequest();
                return await turnRequest.Send(move).GetResponse();
            }
            return new TurnTimedOut(_figureId);
        }
    }
}