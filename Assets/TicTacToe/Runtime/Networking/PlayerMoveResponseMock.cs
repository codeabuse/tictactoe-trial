using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using UnityEngine;

namespace TicTacToe.Networking
{
    public class PlayerMoveResponseMock : IPlayerMoveResponse
    {
        private readonly PlayerMove _requestedMove;

        public PlayerMoveResponseMock(PlayerMove requestedMove)
        {
            _requestedMove = requestedMove;
        }

        public async UniTask<ITurnResult> GetResponse()
        {
            await UniTask.Delay((int)(Random.Range(.08f, 2.06f) * 1000));
            return new TurnSuccesfull(_requestedMove);
        }
    }
}