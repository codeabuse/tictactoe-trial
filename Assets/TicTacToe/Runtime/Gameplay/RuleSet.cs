using System;
using UnityEngine;

namespace TicTacToe.Gameplay
{
    public readonly struct RuleSet
    {
        public readonly Vector2Int BoardDimensions;
        public readonly uint WinningLine;
        public readonly TimeSpan TurnTime;

        public RuleSet(Vector2Int boardDimensions, uint winningLine, TimeSpan turnTime)
        {
            BoardDimensions = boardDimensions;
            WinningLine = winningLine;
            TurnTime = turnTime;
        }
    }
}