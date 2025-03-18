using System;
using System.Threading;
using Bootstrap.Currency;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainMenu.UI.Header
{
    public class MainMenuHeaderManager : IDisposable
    {
        private readonly MainMenuHeader _mainMenuHeaderPrefab;
        private MainMenuHeader _mainMenuHeader;
        private readonly Camera _uiCamera;
        private readonly CurrencyController _currencyController;
        private readonly CancellationToken _cancellationToken;


        public MainMenuHeaderManager(MainMenuHeader mainMenuHeaderPrefab, Camera uiCamera,
            CurrencyController currencyController, CancellationToken cancellationToken)
        {
            _mainMenuHeaderPrefab = mainMenuHeaderPrefab;
            _uiCamera = uiCamera;
            _currencyController = currencyController;
            _cancellationToken = cancellationToken;
        }

        public RectTransform CreateHeader()
        {
            _mainMenuHeader = Object.Instantiate(_mainMenuHeaderPrefab);
            _mainMenuHeader.AssignCamera(_uiCamera);

            _currencyController.InitMainMenuHeaderCurrencies(_mainMenuHeader, _cancellationToken);

            return _mainMenuHeader.transform.GetChild(0).GetComponent<RectTransform>();
        }

        public void Dispose()
        {
            _currencyController.ClearMainMenuHeaderCurrencies();
        }
    }
}