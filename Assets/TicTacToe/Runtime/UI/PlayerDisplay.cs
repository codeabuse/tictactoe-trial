using TicTacToe.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI
{
    public class PlayerDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _name;
        
        [SerializeField]
        private Image _avatar;
        
        [SerializeField]
        private Image _turnMarker;

        private float _initialAlpha;

        private void Awake()
        {
            _initialAlpha = _turnMarker.color.a;
        }

        public void SetPlayer(PlayerEntity playerEntity)
        {
            _name.text = playerEntity.Name;
            _avatar.sprite = playerEntity.Avatar;
        }

        public void ShowTurnMarker()
        {
            var visibleColor = _turnMarker.color;
            visibleColor.a = _initialAlpha;
            _turnMarker.color = visibleColor;
        }

        public void HideTurnMarker()
        {
            var hiddenColor = _turnMarker.color;
            hiddenColor.a = 0;
            _turnMarker.color = hiddenColor;
        }
    }
}