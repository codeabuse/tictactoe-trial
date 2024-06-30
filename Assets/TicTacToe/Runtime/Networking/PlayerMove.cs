namespace TicTacToe.Networking
{
    public readonly struct PlayerMove
    {
        public readonly int PlayerId;
        public readonly int CellX;
        public readonly int CellY;

        public PlayerMove(int playerId, int cellX, int cellY)
        {
            PlayerId = playerId;
            CellX = cellX;
            CellY = cellY;
        }
    }
}