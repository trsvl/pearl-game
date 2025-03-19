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

        public async UniTask Move(RectTransform uiElement, float duration, Vector2 initialPosition = default,
            (float, float) initialOffset = default, Vector2 targetPosition = default,
            (float, float) targetOffset = default)
        {
            targetPosition = targetPosition == default
                ? uiElement.anchoredPosition + new Vector2(targetOffset.Item1, targetOffset.Item2)
                : targetPosition + new Vector2(targetOffset.Item1, targetOffset.Item2);

            uiElement.anchoredPosition = initialPosition == default
                ? uiElement.anchoredPosition + new Vector2(initialOffset.Item1, initialOffset.Item2)
                : initialPosition + new Vector2(initialOffset.Item1, initialOffset.Item2);
            uiElement.gameObject.SetActive(true);

            await uiElement.DOAnchorPos(targetPosition, duration).SetEase(Ease.Linear).SetUpdate(true)
                .ToUniTask(cancellationToken: _cancellationToken);
        }

        public void MoveOnStart()
        {
            const float duration = 0.15f;

            Move(_header, duration, initialOffset: (0f, 500f)).Forget();
            Move(_pauseButton, duration, initialOffset: (500f, 0f)).Forget();
        }
    }
}