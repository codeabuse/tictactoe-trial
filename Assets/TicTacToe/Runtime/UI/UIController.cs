using System;
using TicTacToe.Gameplay;
using TicTacToe.UI;
using TMPro;
using UnityEngine;

namespace TicTacToe
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _timerText;
        
        [SerializeField]
        private string _timeFormat = "00";

        [SerializeField]
        private BoardController _board;

        [SerializeField]
        private PlayerDisplay[] _playerDisplays;

        public void Initialize()
        {
            _board.ActivePlayerChanged += OnActivePlayerChanged;
            _board.TurnTimer.OnReset += OnTimeChanged;
            _board.TurnTimer.OnTick += OnTimeChanged;
            foreach (var playerDisplay in _playerDisplays)
            {
                playerDisplay.HideTurnMarker();
            }
        }

        private void OnTimeChanged(TimeSpan time)
        {
            _timerText.text = time.TotalSeconds.ToString(_timeFormat);
        }

        private void OnActivePlayerChanged(int playerIndex)
        {
            var inactivePlayer = (playerIndex + 1) % 2;
            _playerDisplays[playerIndex].ShowTurnMarker();
            _playerDisplays[inactivePlayer].HideTurnMarker();
        }

        public void ShowPlayers(PlayerEntity[] players)
        {
            for (var i = 0; i < players.Length; i++)
            {
                var playerDisplay = _playerDisplays[i];
                var playerEntity = players[i];
                playerDisplay.SetPlayer(playerEntity);
            }
        }

        public void HideActivePlayerMark()
        {
            foreach (var playerDisplay in _playerDisplays)
            {
                playerDisplay.HideTurnMarker();
            }
        }
    }
}