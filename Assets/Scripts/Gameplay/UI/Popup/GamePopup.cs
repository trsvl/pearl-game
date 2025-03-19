using UnityEngine;
using UnityEngine.Events;
using Utils.UI.Buttons;

namespace Gameplay.UI.Popup
{
    public class GamePopup : MonoBehaviour
    {
        [SerializeField] private TextButton _buttonPrefab;
        [SerializeField] private RectTransform _container;


        public void PauseGame(UnityAction RestartGameClick, UnityAction ResumeGameCLick, UnityAction MainMenuClick)
        {
            AssignButton(ResumeGameCLick, "Resume game");
            AssignButton(RestartGameClick, "Restart game");
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

        public void AssignCamera(Camera canvasCamera)
        {
            GetComponent<Canvas>().worldCamera = canvasCamera;
        }

        public RectTransform GetContainer()
        {
            return _container;
        }

        private void AssignButton(UnityAction listener, string text)
        {
            var button = Instantiate(_buttonPrefab, _container.transform);
            button.Init(listener, text);
        }
    }
}