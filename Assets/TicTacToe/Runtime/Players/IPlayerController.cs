using System.Threading;
using Cysharp.Threading.Tasks;
using TicTacToe.Gameplay;

namespace TicTacToe.Players
{
    public interface IPlayerController
    {
        UniTask<ITurnResult> MakeTurn(CancellationToken ct);
    }
}