using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.Runtime.Gameplay
{
    public class CellViewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private SpriteRenderer _hoverSprite;
        [SerializeField]
        private float _hoverFadeInTime = .1f;
        [SerializeField]
        private float _hoverFadeOutTime = .35f;

        private AsyncPointerClickTrigger _pointerClickTrigger;

        private Cell _model;
        private TweenerCore<Color, Color, ColorOptions> _fadeTween;

        public Cell Model => _model;

        public void Initialize(Cell model)
        {
            _model = model;
            _pointerClickTrigger = gameObject.GetAsyncPointerClickTrigger();
            _hoverSprite.DOFade(0f, 0);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            FadeInHoverSprite();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            FadeOutHoverSprite();
        }

        private void FadeInHoverSprite()
        {
            if (!_hoverSprite)
                return;
        
            _fadeTween = _hoverSprite.DOFade(1f, _hoverFadeInTime);
        }
    
        private void FadeOutHoverSprite()
        {
            if (!_hoverSprite)
                return;

            if (_fadeTween is { })
            {
                _fadeTween.Pause();
                var targetColor = _hoverSprite.color;
                targetColor.a = 0;
                _fadeTween.endValue = targetColor;
            }
            else
            {
                _fadeTween = _hoverSprite.DOFade(0f, _hoverFadeOutTime);
            }
        }

        public async UniTask<CellViewController> WaitCellClicked(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var pointerEventData = await _pointerClickTrigger.OnPointerClickAsync(ct);
                if (pointerEventData.button is PointerEventData.InputButton.Left)
                {
                    Debug.Log($"{_model.Position} clicked");
                    return this;
                }
            }

            return null;
        }
    }
}