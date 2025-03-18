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

        public async UniTask Swap(float duration)
        {
            Vector2 targetPosition = _gameplayHeader.anchoredPosition;

            await _moveUIAnimation.Move(_gameplayHeader, duration, targetOffset: (0f, 500f));
            Object.Destroy(_gameplayHeader.gameObject);

            RectTransform mainMenuHeader = _mainMenuHeaderManager.CreateHeader();
            await _moveUIAnimation.Move(mainMenuHeader, duration, initialOffset: (0f, 500f),
                targetPosition: targetPosition);
        }
    }
}