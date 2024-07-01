using System;
using TicTacToe.Gameplay;
using TicTacToe.Structures;
using UnityEngine;

namespace TicTacToe
{
    public class Cell
    {
        public const int EmptyCellId = -1;
        public Vector2Int Position { get; }
        public int FigureId { get; private set; } = -1;
        
        public bool IsEmpty => FigureId == EmptyCellId;
        public event Action<int> OnFigurePlaced;
        public event Action OnCellCleared;

        public Cell(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }

        public ResultVoid<GameplayException> SetFigure(int figureId)
        {
            if (!IsEmpty)
            {
                return new GameplayException("Non-empty cell can not be used!");
            }
            
            FigureId = figureId;
            OnFigurePlaced?.Invoke(figureId);
            return ResultVoid<GameplayException>.Success;
        }

        public void Clear()
        {
            FigureId = EmptyCellId;
            OnCellCleared?.Invoke();
        }

        public override string ToString()
        {
            return $"{Position}:{(IsEmpty? "empty" : FigureId.ToString())}";
        }
    }
}