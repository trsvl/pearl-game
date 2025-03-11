using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.Animations
{
    public class MoveUIAnimation : IStartAnimation
    {
        private readonly RectTransform _header;
        private readonly RectTransform _pauseButton;


        public MoveUIAnimation(RectTransform header, RectTransform pauseButton)
        {
            _header = header;
            _pauseButton = pauseButton;

            _header.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
        }

        private void MoveToTarget(RectTransform uiElement, float offsetX, float offsetY, float duration,
            Vector2 targetPos = default)
        {
            targetPos = targetPos == default ? uiElement.anchoredPosition : targetPos;
            Vector2 initialPos = targetPos + new Vector2(offsetX, offsetY);
            uiElement.anchoredPosition = initialPos;
            uiElement.gameObject.SetActive(true);

            uiElement.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad);
        }

        public UniTask DoAnimation()
        {
            const float duration = 0.15f;
            
            MoveToTarget(_header, 0, 500f, duration);
            MoveToTarget(_pauseButton, 500f, 0, duration * 2);

            return UniTask.CompletedTask;
        }
    }
}