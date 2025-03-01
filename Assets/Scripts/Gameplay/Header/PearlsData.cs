using System.Text;
using TMPro;

namespace Gameplay.Header
{
    public class PearlsData : IDestroySphere
    {
        private readonly TextMeshProUGUI _pearlsText;
        private readonly StringBuilder textBuilder;
        private int _currentNumber;


        public PearlsData(TextMeshProUGUI pearlsText)
        {
            _pearlsText = pearlsText;
            textBuilder = new StringBuilder();
        }

        private void UpdateText()
        {
            textBuilder.Clear();
            textBuilder.Append($"{_currentNumber}");
            _pearlsText.SetText(textBuilder);
        }

        public void OnDestroySphere()
        {
            _currentNumber += 1;
            UpdateText();
        }
    }
}