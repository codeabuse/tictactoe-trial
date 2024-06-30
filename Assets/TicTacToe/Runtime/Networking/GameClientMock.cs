namespace TicTacToe.Networking
{
    public class GameClientMock : IGameClient
    {
        public IPlayerMoveRequest CreateMoveRequest()
        {
            return new PlayerMoveRequestMock();
        }
    }
}