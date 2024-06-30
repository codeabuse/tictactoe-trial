using System;
using System.Collections.Generic;
using TicTacToe.Runtime.Gameplay;
using TicTacToe.StaticData;
using TicTacToe.Structures;
using UnityEngine;

namespace TicTacToe.Model
{
    public class Board
    {
        public const string OutOfBoundsError = "Position is out of board bounds";
        public const string EmptyCellWinCheckError = "Can't check winning conditions from empty cell!";

        private readonly Dictionary<Vector2Int, Cell> _cells;
        public Dictionary<Vector2Int, Cell> Cells => _cells;
        
        public int Width { get; }
        public int Height { get; }

        public Board(Vector2Int dimensions)
        {
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

        public Result<GameState, string> CheckWinConditions(Vector2Int startFrom)
        {
            if (OutOfBounds(startFrom))
            {
                return OutOfBoundsError;
            }
            
            var cell = _cells[startFrom];
            if (cell.IsEmpty)
            {
                return EmptyCellWinCheckError;
            }

            var figureId = cell.FigureId;
            
            
            foreach (var line in BoardTools.Lines)
            {
                var figuresOnLine = GetLineLenght(figureId, cell, line);
                if (figuresOnLine == DefaultSettings.WinningLineLength)
                    return GameState.GameOver;
            }
            
            return GameState.WaitForNextTurn;
        }

        public int GetLineLenght(int figureId, Cell cell, Vector2Int lineDirection)
        {
            var startPosition = cell.Position;
            var lineLength = GetLineLenghtInDirection(figureId, startPosition, lineDirection);
            
            var oppositeDirtection = lineDirection * -1;
            var nextStartCell = startPosition + oppositeDirtection;
            
            lineLength += GetLineLenghtInDirection(figureId, nextStartCell, oppositeDirtection);
            
            return lineLength;
        }

        private int GetLineLenghtInDirection(int figureId, Vector2Int startPosition, Vector2Int direction)
        {
            var nextPosition = startPosition;
            var figuresOnLine = 0;
            
            while (FigureMatch(nextPosition, figureId))
            {
                figuresOnLine++;
                nextPosition += direction;
            } 

            return figuresOnLine;
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
    }
}