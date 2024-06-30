using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Model.Test
{
    public static class TestBoardArrangements
    {
        public static readonly (
                (Vector2Int position, int figureId)[] board, 
                Vector2Int startPosition, int winningFigure)[] figurePlacements =
        {
                (new (Vector2Int position, int figureId)[]
                {
                        (new(0, 0), 0),
                        (new(1, 1), 0),
                        (new(2, 2), 0),
                        (new(3, 3), 0),
                        (new(4, 4), 0),
                        (new(0, 1), 1),
                        (new(0, 2), 1),
                        (new(0, 3), 1),
                        (new(1, 2), 1),
                        (new(3, 0), 1)
                }, new(0, 0), 0),
                (new (Vector2Int position, int figureId)[]
                {
                        (new(0, 0), 0),
                        (new(0, 1), 0),
                        (new(0, 2), 0),
                        (new(0, 3), 0),
                        (new(0, 4), 0),
                        (new(1, 0), 1),
                        (new(1, 2), 1),
                        (new(2, 3), 1),
                        (new(3, 2), 1),
                        (new(4, 5), 1)
                }, new(0, 0), 0),
                (new (Vector2Int position, int figureId)[]
                {
                        (new(5, 5), 1),
                        (new(4, 4), 1),
                        (new(3, 3), 1),
                        (new(2, 2), 1),
                        (new(1, 1), 1),
                        (new(0, 0), 0),
                        (new(1, 2), 0),
                        (new(2, 3), 0),
                        (new(3, 2), 1),
                        (new(4, 5), 0)
                }, new(1, 1), 1)
        };
        
        

        public static void SetupFigures(Dictionary<Vector2Int, Cell> cells, (Vector2Int position, int figureId)[] figures)
        {
            foreach (var (position, figure) in figures)
            {
                cells[position].SetFigure(figure);
            }
        }
    }
}