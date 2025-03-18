using Bootstrap;
using Cysharp.Threading.Tasks;
using Gameplay.Animations;
using Gameplay.Utils;
using UnityEngine;

namespace Gameplay.UI.Popup
{
    public class GamePopupManager : IPauseGame, IResumeGame, ILoseGame, IFinishGame
    {
        private readonly GamePopup _gamePopupPrefab;
        private readonly Transform _canvases;
        private readonly GameplayStateObserver _gameplayStateObserver;
        private readonly Loader _loader;
        private readonly MoveUIAnimation _moveUIAnimation;
        private readonly CameraController _cameraController;
        private GamePopup _gamePopup;


        public GamePopupManager(GamePopup gamePopupPrefab, Transform canvases,
            GameplayStateObserver gameplayStateObserver, Loader loader, MoveUIAnimation moveUIAnimation,
            CameraController cameraController)
        {
            _gamePopupPrefab = gamePopupPrefab;
            _canvases = canvases;
            _gameplayStateObserver = gameplayStateObserver;
            _loader = loader;
            _moveUIAnimation = moveUIAnimation;
            _cameraController = cameraController;
        }

        public void PauseGame()
        {
            CreatePopup();
            _gamePopup.PauseGame(RestartGameClick, ResumeGameCLick, MainMenuClick);
        }

        public void ResumeGame()
        {
            _gamePopup.ResumeGame();
        }

        public void FinishGame()
        {
            CreatePopup();
            _gamePopup.FinishGame(RestartGameClick, MainMenuClick);
        }

        public void LoseGame()
        {
            CreatePopup();
            _gamePopup.LoseGame(RestartGameClick, MainMenuClick);
        }

        private void CreatePopup()
        {
            _gamePopup = Object.Instantiate(_gamePopupPrefab, _canvases);
            _gamePopup.AssignCamera(_cameraController.GetUICamera());
            _moveUIAnimation.Move(_gamePopup.GetContainer(), 0.25f, initialOffset: (0f, -1000f)).Forget();
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