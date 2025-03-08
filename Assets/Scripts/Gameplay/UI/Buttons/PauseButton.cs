using Gameplay.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Buttons
{
    public class PauseButton : IStartGame
    {
        private readonly Button _button;


        public PauseButton(Button button, GameplayStateObserver gameplayStateObserver)
        {
            _button = button;

            _button.onClick.AddListener(gameplayStateObserver.PauseGame);
            _button.interactable = false;
        }

        public void StartGame()
        {
            _button.interactable = true;
        }
    }
}