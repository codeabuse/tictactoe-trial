using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;

namespace TicTacToe.Networking
{
    public interface IGameClient
    {
        IPlayerMoveRequest CreateMoveRequest();
    }
    
    public interface IPlayerMoveRequest
    {
        IPlayerMoveResponse Send(PlayerMove move);
    }

    public interface IPlayerMoveResponse
    {
        UniTask<ITurnResult> GetResponse();
    }
}