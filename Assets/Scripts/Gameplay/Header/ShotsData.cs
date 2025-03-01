using System.Threading.Tasks;
using TMPro;
using Utils.GameSystemLogic.Installers;

namespace Gameplay.Header
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
                CheckLoseGame();
            }
        }

        private readonly TextMeshProUGUI _shotsText;
        private int _currentNumber;
        private readonly GameplayStateObserver _gameplayStateObserver;


        public ShotsData(TextMeshProUGUI shotsText, int initialNumber, GameplayStateObserver gameplayStateObserver)
        {
            _shotsText = shotsText;
            _currentNumber = initialNumber;
            _gameplayStateObserver = gameplayStateObserver;
        }

        private void UpdateText()
        {
            _shotsText.SetText($"{_currentNumber}");
        }

        private async void CheckLoseGame()
        {
            if (_currentNumber > 0) return;

            await Task.Delay(1000);
            _gameplayStateObserver.LoseGame();
        }
    }
}