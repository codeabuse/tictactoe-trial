namespace TicTacToe.Networking
{
    public static class GameClient
    {
        private static IGameClient _implementation;
        
        internal static void SetClientBackend(IGameClient implementation)
        {
            _implementation = implementation;
        }

        internal static void Connect()
        {
            
        }

        public static IPlayerMoveRequest CreatePlayerMoveRequest()
        {
            return _implementation.CreateMoveRequest();
        }
    }
}