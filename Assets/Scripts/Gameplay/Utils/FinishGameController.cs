using Cysharp.Threading.Tasks;
using Gameplay.Animations;
using Gameplay.UI.Popup;
using Utils.EventBusSystem;

namespace Gameplay.Utils
{
    public class FinishGameController
    {
        private readonly EventBus _eventBus;
        private readonly MoveUIAnimation _moveUIAnimation;
        private readonly GamePopupManager _gamePopupManager;
        private readonly ChangeHeader _changeHeader;

        public FinishGameController(EventBus eventBus, MoveUIAnimation moveUIAnimation,
            GamePopupManager gamePopupManager, ChangeHeader changeHeader)
        {
            _eventBus = eventBus;
            _moveUIAnimation = moveUIAnimation;
            _gamePopupManager = gamePopupManager;
            _changeHeader = changeHeader;
        }

        public async UniTask FinishGame()
        {
            _gamePopupManager.FinishGame();
            _changeHeader.Do(0.5f).Forget();
            
        }
    }
}