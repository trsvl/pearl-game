using Gameplay.Utils;
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
            _button.enabled = false;
        }

        public void StartGame()
        {
            _button.enabled = true;
        }
    }
}