﻿using TicTacToe.Players;
using TicTacToe.Runtime.UI;
using UnityEngine;

namespace TicTacToe.Runtime.Gameplay.GameModes
{
    public class PlayerVsPlayer: MonoBehaviour, IGameMode
    {
        [SerializeField]
        private string player1name = "Player 1";
        [SerializeField]
        private string player2name = "Player 2";

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
                    new PlayerEntity(player1name, _avatars[0], 0,
                            new HumanPlayerController(player1name, _boardController, _popupController)),
                    
                    new PlayerEntity(player2name,_avatars[1], 1,
                            new HumanPlayerController(player2name, _boardController, _popupController))
            };
        }
    }
}