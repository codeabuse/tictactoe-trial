using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.Gameplay
{
    public class CellViewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private SpriteRenderer _hoverSprite;
        [SerializeField]
        private SpriteRenderer _figureSprite;
        [SerializeField]
        private float _hoverFadeInTime = .1f;
        [SerializeField]
        private float _hoverFadeOutTime = .35f;

        private Cell _model;
        private Sprite[] _figures;
        private AsyncPointerClickTrigger _pointerClickTrigger;
        private TweenerCore<Color, Color, ColorOptions> _fadeTween;
        private bool _isHoverable;

        public Cell Model => _model;
        public Vector2Int Position { get; set; }

        public void Initialize(Cell model, Sprite[] figures)
        {
            _figures = figures;
            _model = model;
            _model.OnFigurePlaced += OnFigurePlaced;
            _model.OnCellCleared += OnCellCleared;
            _pointerClickTrigger = gameObject.GetAsyncPointerClickTrigger();
            _hoverSprite.DOFade(0f, 0);
        }

        private void OnFigurePlaced(int figureId)
        {
            _figureSprite.sprite = _figures[figureId];
        }

        private void OnCellCleared()
        {
            _figureSprite.sprite = null;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!_isHoverable)
                return;
            
            FadeInHoverSprite();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            FadeOutHoverSprite();
        }

        private void FadeInHoverSprite()
        {
            _fadeTween?.Kill();
            _fadeTween = _hoverSprite.DOFade(1f, _hoverFadeInTime);
        }
    
        private void FadeOutHoverSprite()
        {
            _fadeTween?.Kill();
            _fadeTween = _hoverSprite.DOFade(0f, _hoverFadeOutTime);
        }

        public async UniTask<CellViewController> WaitCellClicked(CancellationToken ct)
        {
            _isHoverable = true;
            while (!ct.IsCancellationRequested)
            {
                var pointerEventData = await _pointerClickTrigger.OnPointerClickAsync(ct);
                if (pointerEventData.button is PointerEventData.InputButton.Left)
                {
                    Debug.Log($"{_model.Position} clicked");
                    _isHoverable = false;
                    return this;
                }
            }

            _isHoverable = false;
            return null;
        }
    }
}