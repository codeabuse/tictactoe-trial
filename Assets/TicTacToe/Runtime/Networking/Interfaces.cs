using Cysharp.Threading.Tasks;

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
        UniTask<ServerResponse> GetResponse();
    }
}