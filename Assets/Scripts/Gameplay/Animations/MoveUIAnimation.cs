using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.Animations
{
    public class MoveUIAnimation
    {
        private readonly RectTransform _header;
        private readonly RectTransform _pauseButton;
        private readonly CancellationToken _cancellationToken;


        public MoveUIAnimation(RectTransform header, RectTransform pauseButton, CancellationToken cancellationToken)
        {
            _header = header;
            _pauseButton = pauseButton;
            _cancellationToken = cancellationToken;

            _header.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
        }

        public async UniTask Move(RectTransform uiElement, float initialOffsetX, float initialOffsetY, float duration,
            Vector2 targetPos = default)
        {
            targetPos = targetPos == default ? uiElement.anchoredPosition : targetPos;
            Vector2 initialPos = targetPos + new Vector2(initialOffsetX, initialOffsetY);
            uiElement.anchoredPosition = initialPos;
            uiElement.gameObject.SetActive(true);

            await uiElement.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad).SetUpdate(true)
                .ToUniTask(cancellationToken: _cancellationToken);
        }

        public UniTask DoAnimation() //!!!
        {
            const float duration = 0.15f;

            Move(_header, 0, 500f, duration).Forget();
            Move(_pauseButton, 500f, 0, duration * 2).Forget();

            return UniTask.CompletedTask;
        }
    }
}