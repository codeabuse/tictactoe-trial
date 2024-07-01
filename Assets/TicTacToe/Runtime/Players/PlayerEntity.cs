using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TicTacToe.Gameplay
{
    public class PlayerEntity
    {
        private readonly IPlayerController _playerController;
        private int _wins;
        private int _losses;
        private int _draws;

        public string Name { get; }
        public int FigureId { get; private set; }
        public Sprite Avatar { get; set; }

        public int Wins => _wins;
        public int Losses => _losses;
        public int Draws => _draws;

        public PlayerEntity(string name, Sprite avatar, int figureId, IPlayerController controller)
        {
            Name = name;
            Avatar = avatar;
            FigureId = figureId;
            _playerController = controller;
        }
        
        public async UniTask<ITurnResult> WaitForTurn(CancellationToken ct)
        {
            return await _playerController.MakeTurn(ct);
        }

        public void ChangeFigureId(int figureId) => FigureId = figureId;

        public void IncrementWins()
        {
            _wins++;
        }

        public void IncrementLosses()
        {
            _losses++;
        }

        public void IncrementDraws()
        {
            _draws++;
        }
    }
}