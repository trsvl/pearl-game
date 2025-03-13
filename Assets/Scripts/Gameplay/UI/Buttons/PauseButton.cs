using Bootstrap;
using Gameplay.Utils;
using UnityEngine.UI;
using Utils.UI.Buttons;

namespace Gameplay.UI.Buttons
{
    public class PauseButton : BaseButton, IStartGame
    {
        public PauseButton(Button button, GameplayStateObserver gameplayStateObserver, AudioManager audioManager) :
            base(button, audioManager)
        {
            _button.onClick.AddListener(gameplayStateObserver.PauseGame);
            _button.interactable = false;
        }

        public void StartGame()
        {
            _button.interactable = true;
        }
    }
}