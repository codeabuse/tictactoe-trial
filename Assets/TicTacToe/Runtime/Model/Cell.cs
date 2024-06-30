using UnityEngine;

namespace TicTacToe
{
    public class Cell
    {
        public const int EmptyCellId = -1;
        public Vector2Int Position { get; }
        public int FigureId { get; private set; } = -1;
        
        public bool IsEmpty => FigureId == EmptyCellId;

        public Cell(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }

        public void SetFigure(int figureId)
        {
            FigureId = figureId;
        }

        public void Clear()
        {
            FigureId = EmptyCellId;
        }
    }
}