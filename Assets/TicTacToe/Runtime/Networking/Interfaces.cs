using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Structures;

namespace TicTacToe.Networking
{
    public interface IGameClient
    {
        IPlayerMoveRequest CreateMoveRequest();
        IGameStartRequest CreateGameStartRequest();
        IEndGameRequest CreateEndGameRequest();
    }

    public interface IGameStartRequest
    {
        IGameStartResponse Send(PlayerEntity[] players);
    }

    public interface IGameStartResponse
    {
        UniTask<ResultVoid<NetworkError>> GetResponse();
    }

    public interface IPlayerMoveRequest
    {
        IPlayerMoveResponse Send(PlayerMove move);
    }

    public interface IPlayerMoveResponse
    {
        UniTask<ResultVoid<NetworkError>> GetResponse();
    }

    public interface IEndGameRequest
    {
        IEndGameResponse Send(IGameResult result);
    }

    public interface IEndGameResponse
    {
        UniTask<ResultVoid<NetworkError>> GetResponse();
    }
}