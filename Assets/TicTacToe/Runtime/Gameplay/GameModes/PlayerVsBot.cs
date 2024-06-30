using TicTacToe.Players;
using TicTacToe.Runtime.UI;
using UnityEngine;

namespace TicTacToe.Runtime.Gameplay.GameModes
{
    public class PlayerVsBot : MonoBehaviour, IGameMode
    {
        [SerializeField]
        private string humanPlayerName = "Player";
        [SerializeField]
        private string botPlayerName = "Computer";

        [SerializeField]
        private Sprite[] _avatars;
        [SerializeField]
        private  BoardController _boardController;
        [SerializeField]
        private PopupController _popupController;

        public PlayerEntity[] CreatePlayers()
        {
            return new []
            {
                    new PlayerEntity(humanPlayerName, _avatars[0], 0,
                            new HumanPlayerController(humanPlayerName, _boardController, _popupController)),
                    
                    new PlayerEntity(botPlayerName,_avatars[1], 1,
                            new BotPlayer(_boardController.Model, _boardController.Rules, 1, 0))
            };
        }

    }
}