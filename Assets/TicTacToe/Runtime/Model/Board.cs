using System;
using System.Collections.Generic;
using TicTacToe.Gameplay;
using TicTacToe.Structures;
using UnityEngine;

namespace TicTacToe.Model
{
    public class Board
    {
        private readonly RuleSet _rules;
        public const string OutOfBoundsError = "Position is out of board bounds";
        public const string EmptyCellWinCheckError = "Can't check winning conditions from empty cell!";

        private readonly Dictionary<Vector2Int, Cell> _cells;
        public Dictionary<Vector2Int, Cell> Cells => _cells;
        
        public int Width { get; }
        public int Height { get; }

        public Line WinningLine { get; private set; }

        public Board(RuleSet rules)
        {
            _rules = rules;
            var dimensions = rules.BoardDimensions;
            if (dimensions.x <= 0 || dimensions.y <= 0)
                throw new ArgumentOutOfRangeException(nameof(dimensions),"Board dimensions must be greater than zero");
            
            Width = dimensions.x;
            Height = dimensions.y;
            _cells = new (Width * Height);
            for (var w = 0; w < Width; w++)
            {
                for (var h = 0; h < Height; h++)
                {
                    _cells[new(w, h)] = new Cell(w, h);
                }
            }
        }

        public Result<Cell, string> this[Vector2Int position]
        {
            get
            {
                if (_cells.TryGetValue(position, out var cell))
                    return cell;
                return OutOfBoundsError;
            }
        }

        public bool HasNonEmptyNeighbour(Cell cell)
        {
            foreach (var direction in BoardTools.Directions)
            {
                var position = cell.Position + direction;
                if (!_cells.TryGetValue(position, out var neighbour))
                    continue;
                if (!neighbour.IsEmpty)
                    return true;
            }

            return false;
        }

        public void GetNeighbours(Cell target, List<Cell> result)
        {
            result.Clear();
            var start = target.Position;
            
            foreach (var direction in BoardTools.Directions)
            {
                var position = start + direction;
                
                if (!_cells.TryGetValue(position, out var neighbour))
                    continue;
                
                result.Add(neighbour);
            }
        }

        public bool CheckWinConditions(Vector2Int startFrom, out Line winningLine)
        {
            winningLine = default;
            if (OutOfBounds(startFrom))
            {
                return false;
            }
            
            var cell = _cells[startFrom];
            if (cell.IsEmpty)
            {
                return false;
            }

            var figureId = cell.FigureId;
            
            foreach (var lineDirection in BoardTools.LineDirections)
            {
                var line = GetLineLenght(figureId, startFrom, lineDirection, false);
                if (line.FiguresInLine == _rules.WinningLine)
                {
                    winningLine = line;
                    return true;
                }
            }
            
            return false;
        }

        public Line GetLineLenght(int figureId, Vector2Int start, Vector2Int lineDirection, bool excludeStartPoint)
        {
            var startPosition = excludeStartPoint? start + lineDirection : start;
            
            var lineForward = GetLineLenghtInDirection(figureId, startPosition, lineDirection);
            var oppositeDirtection = lineDirection * -1;
            var lineOpposite  = GetLineLenghtInDirection(figureId, start + oppositeDirtection, oppositeDirtection);

            return new Line(
                    lineForward.End, 
                    lineOpposite.End, 
                    lineForward.FiguresInLine + lineOpposite.FiguresInLine);
        }

        private Line GetLineLenghtInDirection(int figureId, Vector2Int startPosition, Vector2Int direction)
        {
            var currentPosition = startPosition;
            var nextPosition = currentPosition;
            var figuresInDirection = 0;

            while (FigureMatch(nextPosition, figureId))
            {
                currentPosition = nextPosition;
                figuresInDirection++;
                nextPosition += direction;
            } 

            return new (startPosition, currentPosition, figuresInDirection);
        }

        private bool OutOfBounds(Vector2Int position)
        {
            return position.x < 0 || position.x >= Width ||
                   position.y < 0 || position.y >= Height;
        }

        private bool FigureMatch(Vector2Int position, int figureId)
        {
            if (OutOfBounds(position))
            {
                return false;
            }
            return _cells[position].FigureId == figureId;
        }

        public void Clear()
        {
            foreach (var cell in _cells.Values)
            {
                cell.Clear();
            }
        }

        public void SetWinningLine(Line winningLine)
        {
            WinningLine = winningLine;
        }
    }
}