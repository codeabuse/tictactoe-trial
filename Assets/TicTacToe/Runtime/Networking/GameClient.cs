
namespace TicTacToe.Networking
{
    public static class GameClient
    {
        private static IGameClient _implementation;
        
        internal static void ApplyConfiguration(ClientConfiguration config)
        {
            _implementation = config.GetClientBackend();
        }

        public static IPlayerMoveRequest CreatePlayerMoveRequest()
        {
            return _implementation.CreateMoveRequest();
        }

        public static IGameStartRequest CreateGameStartRequest()
        {
            return _implementation.CreateGameStartRequest();
        }

        public static IEndGameRequest CreateEndGameRequest()
        {
            return _implementation.CreateEndGameRequest();
        }
    }
}