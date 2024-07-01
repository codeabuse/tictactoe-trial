using UnityEngine;

namespace TicTacToe.Model
{
    public static class BoardTools
    {
        public static readonly Vector2Int[] LineDirections = new[]
        { 
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
        };
        public static readonly Vector2Int[] Directions = new[]
        { 
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
        };
    }
}