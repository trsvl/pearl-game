using Gameplay.SphereData;
using Gameplay.UI.Header;

namespace Gameplay.Utils
{
    public class GameResultChecker : IAfterDestroySphereSegment
    {
        private readonly ShotsData _shotsData;
        private readonly GameplayStateObserver _gameplayStateObserver;
        private readonly SpheresDictionary _spheresDictionary;


        public GameResultChecker(ShotsData shotsData, GameplayStateObserver gameplayStateObserver,
            SpheresDictionary spheresDictionary)
        {
            _shotsData = shotsData;
            _gameplayStateObserver = gameplayStateObserver;
            _spheresDictionary = spheresDictionary;
        }

        public void CheckGameResult(int currentShotsNumber)
        {
            if (_spheresDictionary.GetLevelColors().Length == 0 && currentShotsNumber == _shotsData.CurrentNumber)
            {
                _gameplayStateObserver.FinishGame();
            }
            else if (currentShotsNumber == 0)
            {
                _gameplayStateObserver.LoseGame();
            }
        }

        public void OnAfterDestroySphereSegment(int currentShotsNumber)
        {
            CheckGameResult(currentShotsNumber);
        }
    }
}