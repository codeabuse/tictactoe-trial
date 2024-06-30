using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TicTacToe.Runtime.Gameplay;
using TicTacToe.Runtime.UI;
using TicTacToe.StaticData;

namespace TicTacToe.Players
{
    public class HumanPlayerController : IPlayerController
    {
        private readonly BoardController _board;
        private readonly PopupController _popupController;
        private readonly string _name;

        private CancellationTokenSource _waitCellClickedCancellation;

        public HumanPlayerController(string name, BoardController boardController, PopupController popupController)
        {
            _name = name;
            _popupController = popupController;
            _board = boardController;
        }

        public async UniTask<Vector2Int> MakeTurn(CancellationToken ct)
        {
            await _popupController.ShowAndWaitResponse(
                    GameText.GetPlayerReadyText(_name), 
                    GameText.PlayerReadyButtonText, ct);

            return await _board.WaitUserClicksEmptyCell(ct);
        }
    }
}