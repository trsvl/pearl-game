using UnityEngine;
using UnityEngine.Events;
using Utils.UI.Buttons;

namespace Gameplay.UI.Popup
{
    public class GamePopup : MonoBehaviour
    {
        [SerializeField] private TextButton _buttonPrefab;
        [SerializeField] private Transform buttonSpawnParent;


        public void PauseGame(UnityAction RestartGameClick, UnityAction ResumeGameCLick, UnityAction MainMenuClick)
        {
            AssignButton(RestartGameClick, "Restart game");
            AssignButton(ResumeGameCLick, "Resume game");
            AssignButton(MainMenuClick, "Main menu");
        }

        public void FinishGame(UnityAction RestartGameClick, UnityAction MainMenuClick)
        {
            AssignButton(RestartGameClick, "Play again");
            AssignButton(MainMenuClick, "Main menu");
        }

        public void LoseGame(UnityAction RestartGameClick, UnityAction MainMenuClick)
        {
            AssignButton(RestartGameClick, "Try again");
            AssignButton(MainMenuClick, "Main menu");
        }

        public void ResumeGame()
        {
            Destroy(gameObject);
        }

        private void AssignButton(UnityAction listener, string text)
        {
            var button = Instantiate(_buttonPrefab, buttonSpawnParent);
            button.Set(listener, text);
        }
    }
}