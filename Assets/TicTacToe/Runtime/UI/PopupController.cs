using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TicTacToe.AsyncTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.UI
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _messageBox;

        [SerializeField]
        private RectTransform _popupVisibleTarget;

        [SerializeField]
        private RectTransform _popupHiddenTarget;

        [SerializeField]
        private TMP_Text _message;

        [SerializeField]
        private TMP_Text _buttonText;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private Ease _showEase;
        
        [SerializeField, Range(0, 2)]
        private float _showTime = .7f;
        
        [SerializeField]
        private Ease _hideEase;
        
        [SerializeField, Range(0, 2)]
        private float _hideTime = .7f;

        private UniTaskHandle _popupHideHandle;

        public async UniTask ShowAndWaitResponse(string message, string buttonText, CancellationToken ct = default)
        {
            await ShowAndWaitResponse(message, buttonText, null, ct);
        }
        
        public async UniTask ShowAndWaitResponse(string message, string buttonText, Action callback, CancellationToken ct = default)
        {
            _message.text = message;
            _buttonText.text = buttonText;
            var visiblePosition = _popupVisibleTarget.position;
            await _messageBox.DOMove(visiblePosition, _showTime).SetEase(_showEase).ToUniTask(cancellationToken: ct);
            
            //await (_popupClickCancellation.Task = _button.GetAsyncPointerClickTrigger().OnPointerClickAsync());
            await _button.GetAsyncPointerClickTrigger().OnPointerClickAsync(ct);
            callback?.Invoke();
            _popupHideHandle.Task = Hide(ct);
        }

        public async UniTask Hide(CancellationToken ct = default)
        {
            var hiddenPosition = _popupHiddenTarget.position;
            await _messageBox.DOMove(hiddenPosition, _hideTime).SetEase(_hideEase).ToUniTask(cancellationToken: ct);
        }
    }
}