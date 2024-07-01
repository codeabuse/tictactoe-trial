namespace TicTacToe.Networking
{
    public class PlayerMoveRequestMock : IPlayerMoveRequest
    {
        public IPlayerMoveResponse Send(PlayerMove move)
        {
            return new PlayerMoveResponseMock(move);
        }
    }
}