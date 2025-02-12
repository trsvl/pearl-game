using TMPro;

namespace Gameplay.Header
{
    public class ShotsData
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

        private readonly TextMeshProUGUI _shotsText;
        private int count;


        public ShotsData(TextMeshProUGUI shotsText, int initialCount)
        {
            _shotsText = shotsText;
            Count = initialCount;
        }

        public void UpdateText()
        {
            _shotsText.SetText($"{count}");
        }
    }
}