using Bootstrap;
using UnityEngine.UI;

namespace Utils.UI.Buttons
{
    public class BaseButton
    {
        protected readonly Button _button;


        protected BaseButton(Button button, AudioController audioController)
        {
            _button = button;
            _button.onClick.AddListener(() => audioController.Play(AudioAction.ButtonClick));
        }
    }
}