using Bootstrap;
using Gameplay.Utils;
using UnityEngine;

namespace Gameplay.UI.Popup
{
    public class GamePopupManager : IPauseGame, IResumeGame, IFinishGame, ILoseGame
    {
        private readonly GamePopup _gamePopupPrefab;
        private readonly Transform _canvases;
        private readonly GameplayStateObserver _gameplayStateObserver;
        private readonly Loader _loader;
        private GamePopup _gamePopup;


        public GamePopupManager(GamePopup gamePopupPrefab, Transform canvases,
            GameplayStateObserver gameplayStateObserver, Loader loader)
        {
            _gamePopupPrefab = gamePopupPrefab;
            _canvases = canvases;
            _gameplayStateObserver = gameplayStateObserver;
            _loader = loader;
        }

        public void PauseGame()
        {
            _gamePopup = Object.Instantiate(_gamePopupPrefab, _canvases);
            _gamePopup.PauseGame(RestartGameClick, ResumeGameCLick, MainMenuClick);
        }

        public void ResumeGame()
        {
            _gamePopup.ResumeGame();
        }

        public void FinishGame()
        {
            _gamePopup = Object.Instantiate(_gamePopupPrefab, _canvases);
            _gamePopup.FinishGame(RestartGameClick, MainMenuClick);
        }

        public void LoseGame()
        {
            _gamePopup = Object.Instantiate(_gamePopupPrefab, _canvases);
            _gamePopup.LoseGame(RestartGameClick, MainMenuClick);
        }

        private void ResumeGameCLick()
        {
            _gameplayStateObserver.ResumeGame();
        }

        private void RestartGameClick()
        {
            _ = _loader.LoadScene(SceneName.Gameplay);
        }

        private void MainMenuClick()
        {
            _ = _loader.LoadScene(SceneName.MainMenu);
        }
    }
}