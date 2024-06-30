using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Model
{
    public readonly struct ReadOnlyBoard
    {
        private static readonly Dictionary<int, char> figures = new()
        {
                { -1, '_' },
                { 0, 'X' },
                { 1, 'O' }
        };

        private const char separator = '|';

        private readonly int[,] _cells;

        public int[,] Cells => _cells;

        public ReadOnlyBoard(Board board)
        {
            _cells = new int[board.Width, board.Height];
            foreach (var cell in board.Cells.Values)
            {
                var (x, y) = (cell.Position.x, cell.Position.y);
                _cells[x, y] = cell.FigureId;
            }
        }

        public override string ToString()
        {
            var width = _cells.GetUpperBound(0);
            var height = _cells.GetUpperBound(1) - _cells.GetLowerBound(1);
            var sb = new StringBuilder((width * 2 + 1) * (height + 1));

            for (var x = 0; x < width * 2 + 1; x++)
            {
                sb.Append('_'); // top border
            }
            
            for (var h = 0; h < height; h++)
            {
                sb.AppendLine().Append(separator);
                for (var w = 0; w < width; w++)
                {
                    sb.Append(figures[_cells[w, h]])
                           .Append(separator);
                }
            }
            
            return sb.ToString();
        }
    }
}