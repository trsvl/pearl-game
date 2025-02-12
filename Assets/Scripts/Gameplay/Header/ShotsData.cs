using System;
using System.Threading.Tasks;
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
                CheckLoseGame();
            }
        }

        private readonly TextMeshProUGUI _shotsText;
        private int count;
        private readonly GameplayStateObserver _gameplayStateObserver;


        public ShotsData(TextMeshProUGUI shotsText, int initialCount, GameplayStateObserver gameplayStateObserver)
        {
            _shotsText = shotsText;
            Count = initialCount;
            _gameplayStateObserver = gameplayStateObserver;
        }

        private void UpdateText()
        {
            _shotsText.SetText($"{count}");
        }

        private async void CheckLoseGame()
        {
            try
            {
                if (count < 1)
                {
                    await LoseGameWithDelay();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task LoseGameWithDelay()
        {
            await Task.Delay(4000);
            LoseGame();
        }

        private void LoseGame()
        {
            _gameplayStateObserver.LoseGame();
        }
    }
}