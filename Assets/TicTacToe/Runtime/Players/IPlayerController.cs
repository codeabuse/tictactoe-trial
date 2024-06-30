using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TicTacToe
{
    public interface IPlayerController
    {
        UniTask<Vector2Int> MakeTurn(CancellationToken ct);
    }
}