using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TicTacToe.Networking
{
    public class PlayerMoveResponseMock : IPlayerMoveResponse
    {
        public async UniTask<ServerResponse> GetResponse()
        {
            await UniTask.Delay((int)(Random.Range(.08f, 2.06f) * 1000));
            return ServerResponse.OK;
        }
    }
}