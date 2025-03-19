using TMPro;

namespace Gameplay.UI.Header
{
    public class ShotsData
    {
        public int CurrentNumber
        {
            get => _currentNumber;
            set
            {
                _currentNumber = value;
                UpdateText();
            }
        }

        private readonly TextMeshProUGUI _shotsText;
        private int _currentNumber;


        public ShotsData(TextMeshProUGUI shotsText)
        {
            _shotsText = shotsText;
        }

        public void SetInitialNumber(int initialNumber)
        {
            CurrentNumber = initialNumber;
        }

        private void UpdateText()
        {
            _shotsText.SetText($"{_currentNumber}");
        }
    }
}