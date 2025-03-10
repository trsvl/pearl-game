using Gameplay.SphereData;
using Gameplay.UI.Header;

namespace Gameplay.Utils
{
    public class GameResultChecker
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

        public void CheckGameResult()
        {
            if (_spheresDictionary.GetLevelColors().Length == 0)
            {
                _gameplayStateObserver.FinishGame();
            }
            else if (_shotsData.CurrentNumber == 0)
            {
                _gameplayStateObserver.LoseGame();
            }
        }
    }
}