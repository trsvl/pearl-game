using Cysharp.Threading.Tasks;
using MainMenu.UI.Header;
using UnityEngine;

namespace Gameplay.Animations
{
    public class ChangeHeader
    {
        private readonly RectTransform _gameplayHeader;
        private readonly MainMenuHeaderManager _mainMenuHeaderManager;
        private readonly MoveUIAnimation _moveUIAnimation;

        public ChangeHeader(RectTransform gameplayHeader, MainMenuHeaderManager mainMenuHeaderManager,
            MoveUIAnimation moveUIAnimation)
        {
            _gameplayHeader = gameplayHeader;
            _mainMenuHeaderManager = mainMenuHeaderManager;
            _moveUIAnimation = moveUIAnimation;
        }

        public async UniTask Do(float duration)
        {
            var mainMenuHeader = _mainMenuHeaderManager.CreateHeader();

            RectTransform newHeader = mainMenuHeader.GetComponentInChildren<RectTransform>();
            Vector2 targetPosition = _gameplayHeader.anchoredPosition;
            _gameplayHeader.gameObject.SetActive(false);
            await _moveUIAnimation.Move(newHeader, 0, 500f, duration, targetPosition);
        }
    }
}