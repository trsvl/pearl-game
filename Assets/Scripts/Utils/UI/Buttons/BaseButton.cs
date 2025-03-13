using Bootstrap;
using UnityEngine.UI;

namespace Utils.UI.Buttons
{
    public class BaseButton
    {
        protected readonly Button _button;


        protected BaseButton(Button button, AudioManager audioManager)
        {
            _button = button;
            _button.onClick.AddListener(() => audioManager.Play(AudioAction.ButtonClick));
        }
    }
}