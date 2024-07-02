using UnityEngine;

namespace TicTacToe.Networking
{
    public readonly struct PlayerMove
    {
        public readonly int PlayerId;
        public readonly int CellX;
        public readonly int CellY;

        public PlayerMove(int playerId, Vector2Int position)
        {
            PlayerId = playerId;
            CellX = position.x;
            CellY = position.y;
        }

        public Vector2Int Position => new(CellX, CellY);

        public override string ToString()
        {
            return $"{PlayerId}: ({CellX},{CellY})";
        }
    }
}