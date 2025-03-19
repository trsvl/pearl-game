using TMPro;
using UnityEngine.Events;

namespace Utils.UI.Buttons
{
    public class TextButton : BaseButton
    {
        private TextMeshProUGUI _text;


        public void Init(UnityAction listener, string text)
        {
            if (!_text) _text = GetComponentInChildren<TextMeshProUGUI>(true);

            Init(listener);
            _text.SetText(text);
        }
    }
}