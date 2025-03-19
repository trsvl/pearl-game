using Bootstrap;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils.UI.Buttons
{
    public class BaseButton : Button
    {
        public void Init(UnityAction listener)
        {
            onClick.AddListener(() => AudioController.Instance.Play(AudioAction.ButtonClick));
            onClick.AddListener(listener);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            onClick.RemoveAllListeners();
        }
    }
}