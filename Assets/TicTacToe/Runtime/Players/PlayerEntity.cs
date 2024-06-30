using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TicTacToe
{
    public class PlayerEntity
    {
        private readonly IPlayerController _playerController;

        public string Name { get; }
        public int FigureId { get; }
        public Sprite Avatar { get; set; }

        public PlayerEntity(string name, Sprite avatar, int figureId, IPlayerController controller)
        {
            Name = name;
            Avatar = avatar;
            FigureId = figureId;
            _playerController = controller;
        }
        
        public async UniTask<Vector2Int> WaitForTurn(CancellationToken ct)
        {
            return await _playerController.MakeTurn(ct);
        }
    }
}