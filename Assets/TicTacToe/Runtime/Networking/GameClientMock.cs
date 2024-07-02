using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using TicTacToe.Structures;

namespace TicTacToe.Networking
{
    public class GameClientMock : IGameClient
    {
        public IPlayerMoveRequest CreateMoveRequest()
        {
            return new PlayerMoveRequestMock();
        }

        public IGameStartRequest CreateGameStartRequest()
        {
            return new GameStartRequestMock();
        }

        public IEndGameRequest CreateEndGameRequest()
        {
            return new GameEndRequestMock();
        }
    }
    
    public class PlayerMoveResponseMock : IPlayerMoveResponse
    {
        private readonly PlayerMove _requestedMove;

        public PlayerMoveResponseMock(PlayerMove requestedMove)
        {
            _requestedMove = requestedMove;
        }

        public async UniTask<ResultVoid<NetworkError>> GetResponse()
        {
            return ResultVoid<NetworkError>.Success;
        }
    }

    public class PlayerMoveRequestMock : IPlayerMoveRequest
    {
        public IPlayerMoveResponse Send(PlayerMove move)
        {
            return new PlayerMoveResponseMock(move);
        }
    }

    public class GameEndRequestMock : IEndGameRequest
    {
        public IEndGameResponse Send(IGameResult result)
        {
            return new GameEndResponseMock();
        }
    }

    public class GameEndResponseMock : IEndGameResponse
    {
        public async UniTask<ResultVoid<NetworkError>> GetResponse()
        {
            return ResultVoid<NetworkError>.Success;
        }
    }

    public class GameStartRequestMock : IGameStartRequest
    {
        public IGameStartResponse Send(PlayerEntity[] players)
        {
            return new GameStartResposneMock(players);
        }
    }

    public class GameStartResposneMock : IGameStartResponse
    {
        public GameStartResposneMock(PlayerEntity[] players)
        {
            
        }

        public async UniTask<ResultVoid<NetworkError>> GetResponse()
        {
            return ResultVoid<NetworkError>.Success;
        }
    }
}