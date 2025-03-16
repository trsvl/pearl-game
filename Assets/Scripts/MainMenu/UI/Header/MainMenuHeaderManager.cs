using Gameplay.Animations;
using UnityEngine;
using VContainer;

namespace MainMenu.UI.Header
{
    public class MainMenuHeaderManager
    {
        private readonly MainMenuHeader _mainMenuHeaderPrefab;
        private MainMenuHeader _mainMenuHeader;
        private readonly Camera _uiCamera;

        public MainMenuHeaderManager(MainMenuHeader mainMenuHeaderPrefab, Camera uiCamera)
        {
            _mainMenuHeaderPrefab = mainMenuHeaderPrefab;
            _uiCamera = uiCamera;
        }


        [Inject]
        public void AssignCreatedHeader()
        {
            _mainMenuHeader = Object.Instantiate(_mainMenuHeaderPrefab);
            _mainMenuHeader.AssignCamera(_uiCamera);
        }

        public MainMenuHeader GetHeader()
        {
            return _mainMenuHeader;
        }
    }
}