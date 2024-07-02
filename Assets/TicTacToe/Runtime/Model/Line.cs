using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Model
{
    public class Line
    {
        public readonly int FigureId;
        private List<Vector2Int> _figures = new();
        private bool _isArranged;

        public Vector2Int Start
        {
            get
            {
                Arrange();
                return Length > 0 ? _figures[0] : default;
            }
        }

        public Vector2Int End { 
            get
            {
                Arrange();
                return Length > 0 ? _figures[^1] : default;
            } 
        }

        public Line(int figureId)
        {
            FigureId = figureId;
        }

        public int Length => _figures.Count;

        public override string ToString()
        {
            return $"{Start}-{End} ({Length})";
        }

        public void AddCell(Vector2Int position)
        {
            _figures.Add(position);
            _isArranged = false;
        }

        private void Arrange()
        {
            if (!_isArranged)
            {
                _figures.Sort((a, b) => a.x - b.x);
                _figures.Sort((a, b) => a.y - b.y);
                _isArranged = true;
            }
        }
    }
}