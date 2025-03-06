using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.Animations
{
    public class MoveUIAnimation : IStartAnimation
    {
        private readonly RectTransform _header;
        private readonly RectTransform _buttons;


        public MoveUIAnimation(RectTransform header, RectTransform buttons)
        {
            _header = header;
            _buttons = buttons;

            _header.gameObject.SetActive(false);
            _buttons.gameObject.SetActive(false);
        }

        private void MoveToTarget(RectTransform uiElement, float offsetX, float offsetY, float duration)
        {
            Vector2 targetPos = uiElement.anchoredPosition;
            Vector2 initialPos = uiElement.anchoredPosition + new Vector2(offsetX, offsetY);
            uiElement.anchoredPosition = initialPos;
            uiElement.gameObject.SetActive(true);

            uiElement.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad);
        }

        public Task DoAnimation()
        {
            MoveToTarget(_header, 0, 500f, 0.5f);
            MoveToTarget(_buttons, 500f, 0, 1f);

            return Task.CompletedTask;
        }
    }
}