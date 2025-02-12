using System.Text;
using TMPro;

namespace Gameplay.Header
{
    public class PearlsData
    {
        public int Count
        {
            get => count;
            set
            {
                count = value;
                UpdateText();
            }
        }

        private readonly TextMeshProUGUI _pearlsText;
        private readonly StringBuilder textBuilder;
        private int count;


        public PearlsData(TextMeshProUGUI pearlsText)
        {
            _pearlsText = pearlsText;
            textBuilder = new StringBuilder();
        }

        private void UpdateText()
        {
            textBuilder.Clear();
            textBuilder.Append($"{count}");
            _pearlsText.SetText(textBuilder);
        }
    }
}