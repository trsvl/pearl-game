using Bootstrap;
using Bootstrap.Currency;
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
        private readonly CurrencyModel _currencyModel;
        private readonly CameraManager _cameraManager;

        public ChangeHeader(RectTransform header, MainMenuHeader mainMenuHeaderPrefab, MoveUIAnimation moveUIAnimation,
            CurrencyModel currencyModel, CameraManager cameraManager)
        {
            _header = header;
            _mainMenuHeaderPrefab = mainMenuHeaderPrefab;
            _moveUIAnimation = moveUIAnimation;
            _currencyModel = currencyModel;
            _cameraManager = cameraManager;
        }

        public async UniTask Do(float duration)
        {
            var mainMenuHeader = Object.Instantiate(_mainMenuHeaderPrefab);

            mainMenuHeader.AssignCamera(_cameraManager.GetUICamera());

            _currencyModel.BindGoldCurrencyText(mainMenuHeader._goldText);
            _currencyModel.BindDiamondCurrencyText(mainMenuHeader._diamondText);
            _currencyModel.BindGoldIcon(mainMenuHeader._goldIcon);
            _currencyModel.BindDiamondIcon(mainMenuHeader._diamondIcon);

            RectTransform newHeader = mainMenuHeader.GetComponentInChildren<RectTransform>();
            Vector2 targetPosition = _header.anchoredPosition;
            _header.gameObject.SetActive(false);
            await _moveUIAnimation.Move(newHeader, 0, 500f, duration, targetPosition);
        }
    }
}