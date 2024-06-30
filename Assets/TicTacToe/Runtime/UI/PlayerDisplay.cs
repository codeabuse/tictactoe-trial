using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Runtime.UI
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

        public void SetName(string name) => _name.text = name;
        
        public void SetAvatar(Sprite sprite) => _avatar.sprite = sprite;

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