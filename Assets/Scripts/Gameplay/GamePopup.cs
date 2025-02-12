using _Project.Scripts.Utils.UI.Buttons;
using UnityEngine;
using UnityEngine.Events;
using Utils.Loader;

namespace Gameplay
{
    public class GamePopup : IPauseGame, IResumeGame, IFinishGame, ILoseGame
    {
        private readonly TextButton _firstButton;
        private readonly TextButton _secondButton;
        private readonly GameplayStateObserver _gameplayStateObserver;
        private readonly GameObject _popup;


        public GamePopup(GameObject popup, TextButton firstButton, TextButton secondButton,
            GameplayStateObserver gameplayStateObserver)
        {
            _popup = popup;
            _firstButton = firstButton;
            _secondButton = secondButton;
            _gameplayStateObserver = gameplayStateObserver;
        }

        public void PauseGame()
        {
            _popup.SetActive(true);
            AssignButton(_firstButton, ResumeGameCLick, "Resume game");
            AssignButton(_secondButton, MainMenuClick, "Main menu");
        }

        public void FinishGame()
        {
            _popup.SetActive(true);
            AssignButton(_firstButton, RestartGameClick, "Play again");
            AssignButton(_secondButton, MainMenuClick, "Main menu");
        }

        public void LoseGame()
        {
            _popup.SetActive(true);
            AssignButton(_firstButton, RestartGameClick, "Try again");
            AssignButton(_secondButton, MainMenuClick, "Main menu");
        }

        public void ResumeGame()
        {
            _popup.SetActive(false);
            _firstButton.RemoveListeners();
            _secondButton.RemoveListeners();
        }

        private void AssignButton(TextButton button, UnityAction listener, string text)
        {
            button.Set(listener, text);
        }

        private void ResumeGameCLick()
        {
            _gameplayStateObserver.ResumeGame();
        }

        private void RestartGameClick()
        {
            _ = Loader.Instance.LoadScene(SceneName.Gameplay);
        }

        private void MainMenuClick()
        {
            _ = Loader.Instance.LoadScene(SceneName.MainMenu);
        }
    }
}