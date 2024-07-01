using UnityEngine;

namespace TicTacToe.Model
{
    public readonly struct Line
    {
        public readonly Vector2Int Start;
        public readonly Vector2Int End;
        public readonly int FiguresInLine;

        public Line(Vector2Int start, Vector2Int end, int figuresInLine)
        {
            Start = start;
            End = end;
            FiguresInLine = figuresInLine;
        }

        public override string ToString()
        {
            return $"{Start}-{End} ({FiguresInLine})";
        }
    }
}