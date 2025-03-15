using Bootstrap;
using Cysharp.Threading.Tasks;
using MainMenu.UI.Header;
using UnityEngine;
using VContainer;

namespace Gameplay.Animations
{
    public class ChangeHeader
    {
        private readonly RectTransform _header;
        private readonly MainMenuHeader _mainMenuHeaderPrefab;
        private readonly MoveUIAnimation _moveUIAnimation;
        private readonly Currencies _currencies;
        private readonly CameraManager _cameraManager;

        public ChangeHeader(RectTransform header, MainMenuHeader mainMenuHeaderPrefab, MoveUIAnimation moveUIAnimation,
            Currencies currencies, CameraManager cameraManager)
        {
            _header = header;
            _mainMenuHeaderPrefab = mainMenuHeaderPrefab;
            _moveUIAnimation = moveUIAnimation;
            _currencies = currencies;
            _cameraManager = cameraManager;
        }

        public async UniTask Do(float duration)
        {
            var mainMenuHeader = Object.Instantiate(_mainMenuHeaderPrefab);

            mainMenuHeader.AssignCamera(_cameraManager.GetUICamera());

            _currencies.BindGoldCurrencyText(mainMenuHeader._goldText);
            _currencies.BindDiamondCurrencyText(mainMenuHeader._diamondText);
            _currencies.BindGoldIcon(mainMenuHeader._goldIcon);
            _currencies.BindDiamondIcon(mainMenuHeader._diamondIcon);

            RectTransform newHeader = mainMenuHeader.GetComponentInChildren<RectTransform>();
            Vector2 targetPosition = _header.anchoredPosition;
            _header.gameObject.SetActive(false);
            await _moveUIAnimation.Move(newHeader, 0, 500f, duration, targetPosition);
        }
    }
}