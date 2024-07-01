using NUnit.Framework;
using TicTacToe;
using TicTacToe.Gameplay;
using TicTacToe.Model;
using TicTacToe.Model.Test;
using TicTacToe.StaticData;
using UnityEngine;
using UnityEngine.Pool;

public class Tests
{
    private Board _board;
    private RuleSet _rules;

    [OneTimeSetUp]
    public void PrepareBoard()
    {
        _rules = DefaultSettings.DefaultRules;
        _board = new Board(_rules);
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _board = null;
    }
    
    [Test]
    public void BoardModel()
    {
        var dimensions = _rules.BoardDimensions;
        var expectedCells = dimensions.x * dimensions.y;
        Assert.AreEqual(_board.Cells.Values.Count, expectedCells);
        
        foreach (var (position, cell) in _board.Cells)
        {
            Assert.NotNull(cell);
            Assert.AreEqual(position, cell.Position);
        }
    }

    [Test]
    public void NeighbourCells()
    {
        var neighbours = ListPool<Cell>.Get();
        var positionsAndNeighbours = new (Vector2Int position, int expectedNeighboursCount)[]
        {
                (new(0, 0), 3),
                (new(1, 1), 8),
                (new(_rules.BoardDimensions.x - 1, _rules.BoardDimensions.y - 1), 3),
                (new(-2, 0), 0)
        };
        
        Cell cell = default;
        foreach (var (position, expectedNeighboursCount) in positionsAndNeighbours)
        {
            var result = _board[position];
            result.Map(
                    c => cell = c, 
                    error => Assert.AreEqual(Board.OutOfBoundsError, error));
            
            if (result.IsFailed)
                continue;
            _board.GetNeighbours(cell, neighbours);
            Assert.AreEqual(expectedNeighboursCount, neighbours.Count);
        }
        
        ListPool<Cell>.Release(neighbours);
    }

    [Test]
    public void WinConditions()
    {
        for (var i = 0; i < TestBoardArrangements.figurePlacements.Length; i++)
        {
            var (board, startPosition, winningFigure) = TestBoardArrangements.figurePlacements[i];
            _board.Clear();
            TestBoardArrangements.SetupFigures(_board.Cells, board);
            if (_board.CheckWinConditions(startPosition, out var winningLine))
            {
                var winnerCell = _board.Cells[winningLine.Start];
                Assert.AreEqual(winningFigure, winnerCell.FigureId);
            }
            else
            {
                Assert.Fail($"Board arrangement {i}: player {winningFigure} win expected");
            }
        }
    }
}
