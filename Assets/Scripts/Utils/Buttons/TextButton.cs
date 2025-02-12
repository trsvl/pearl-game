using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.Utils.UI.Buttons
{
    public class TextButton : Button
    {
        private TextMeshProUGUI _text;


        public void Set(UnityAction listener, string text)
        {
            if (!_text) _text = GetComponentInChildren<TextMeshProUGUI>(true);

            onClick.AddListener(listener);
            _text.SetText(text);
        }

        public void RemoveListeners()
        {
            onClick.RemoveAllListeners();
        }
    }
}