using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Animations
{
    public class ChangeHeader
    {
        private readonly RectTransform _header;
        private readonly GameObject _mainMenuHeaderPrefab;
        private readonly MoveUIAnimation _moveUIAnimation;

        public ChangeHeader(RectTransform header, GameObject mainMenuHeaderPrefab, MoveUIAnimation moveUIAnimation)
        {
            _header = header;
            _mainMenuHeaderPrefab = mainMenuHeaderPrefab;
            _moveUIAnimation = moveUIAnimation;
        }

        public async UniTask Do(float duration)
        {
            var gameObj = Object.Instantiate(_mainMenuHeaderPrefab, _header.parent.transform);
            RectTransform newHeader = gameObj.GetComponent<RectTransform>();
            Vector2 targetPosition = _header.anchoredPosition;
            Object.Destroy(_header.gameObject);
            await _moveUIAnimation.Move(newHeader, 0, 500f, duration, targetPosition);
        }
    }
}