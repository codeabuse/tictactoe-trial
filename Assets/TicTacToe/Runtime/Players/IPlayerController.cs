using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;
using UnityEngine;

namespace TicTacToe
{
    public interface IPlayerController
    {
        UniTask<ITurnResult> MakeTurn(CancellationToken ct);
    }
}