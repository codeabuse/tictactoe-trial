using System;
using TicTacToe.Gameplay;
using TicTacToe.StaticData;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace TicTacToe
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField]
        private Transform _gridRoot;
        
        [SerializeField]
        private CellViewController cellViewPrefab;

        [SerializeField, Range(3, 25)]
        private int _width = DefaultSettings.GameFieldWidth;
        
        [SerializeField, Range(3, 25)]
        private int _height = DefaultSettings.GameFieldHeight;
        
        [SerializeField]
        private float _cellSize = 1f;
        
        [SerializeField]
        private float _gapSize = 0.07f;

        [SerializeField]
        private CellViewController[] _cells;

        [ContextMenu("Generate")]
        public CellViewController[] Generate()
        {
            return Generate(_width, _height);
        }
        
        public CellViewController[] Generate(int width, int height)
        {
            Clear();
            
            var rootPosition = _gridRoot.position;
            var totalHeight = _cellSize * _height + _gapSize * (_height - 1); 
            var totalWidth = _cellSize * _width + _gapSize * (_width - 1);
            var spacing = _cellSize + _gapSize;
            var centerOffset = new Vector3(totalWidth * 0.5f - spacing * 0.5f, totalHeight * 0.5f - spacing * 0.5f);
            
            _cells = new CellViewController[width * height];
            
            for (var h =  0; h < _height; h++)
            {
                for (var w = 0; w < _width; w++)
                {
                    var position = rootPosition + new Vector3(w * spacing, h * spacing) - centerOffset;
                    var cellInstance = Instantiate(cellViewPrefab, position, Quaternion.identity, _gridRoot);
                    _cells[h * _width + w] = cellInstance;
                    cellInstance.Position = new(w, h);
                }
            }

            var length = _width * _height;
            var cellsCopy = new CellViewController[length];
            _cells.CopyTo(cellsCopy, 0);
            
            return cellsCopy;
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            if (_cells is null)
                return;

            Action<UObject> destroyAction = Application.isEditor ? DestroyImmediate : Destroy;
            foreach (var cell in _cells)
            {
                if (!cell) continue;
                destroyAction(cell.gameObject);
            }

            _cells = Array.Empty<CellViewController>();
        }
    }
}