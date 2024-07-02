using TicTacToe.Players;
using TicTacToe.StaticData;
using UnityEngine;

namespace TicTacToe.Gameplay.GameModes
{
    public class PlayerVsBot : MonoBehaviour, IGameMode
    {
        [SerializeField]
        private string humanPlayerName = "Player";
        [SerializeField]
        private string botPlayerName = "Computer";

        [SerializeField]
        private float _minBotIdle = DefaultSettings.BotTurnMinIdle;
        [SerializeField]
        private float _maxBotIdle = DefaultSettings.BotTurnMaxIdle;

        [SerializeField]
        private Sprite[] _avatars;
        [SerializeField]
        private  BoardController _boardController;

        public PlayerEntity[] CreatePlayers()
        {
            return new []
            {
                    new PlayerEntity(humanPlayerName, _avatars[0], 0,
                            new HumanPlayer(humanPlayerName, 0, _boardController)),
                    
                    new PlayerEntity(botPlayerName,_avatars[1], 1,
                            new BotPlayer(_boardController, 1, 0, _minBotIdle, _maxBotIdle))
            };
        }

    }
}