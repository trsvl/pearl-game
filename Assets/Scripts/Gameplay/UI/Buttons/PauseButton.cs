using Gameplay.Utils;
using Utils.UI.Buttons;

namespace Gameplay.UI.Buttons
{
    public class PauseButton : IStartGame, IFinishGame, ILoseGame
    {
        private readonly BaseButton _button;


        public PauseButton(BaseButton button, GameplayStateObserver gameplayStateObserver)
        {
            _button = button;
            _button.Init(gameplayStateObserver.PauseGame);
            _button.interactable = false;
        }

        public void StartGame()
        {
            _button.interactable = true;
        }

        public void FinishGame()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void LoseGame()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}