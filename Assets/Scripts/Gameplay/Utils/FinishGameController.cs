using Bootstrap;
using Cysharp.Threading.Tasks;
using Gameplay.Animations;
using Gameplay.UI.Popup;
using Utils.EventBusSystem;

namespace Gameplay.Utils
{
    public class FinishGameController : IFinishGame
    {
        private readonly EventBus _eventBus;
        private readonly MoveUIAnimation _moveUIAnimation;
        private readonly GamePopupManager _gamePopupManager;
        private readonly ChangeHeader _changeHeader;
        private readonly Currencies _currencies;

        public FinishGameController(EventBus eventBus, MoveUIAnimation moveUIAnimation,
            GamePopupManager gamePopupManager, ChangeHeader changeHeader, Currencies currencies)
        {
            _eventBus = eventBus;
            _moveUIAnimation = moveUIAnimation;
            _gamePopupManager = gamePopupManager;
            _changeHeader = changeHeader;
            _currencies = currencies;
        }

        public void FinishGame()
        {
            _changeHeader.Do(0.5f).Forget();
            _currencies.GoldCurrency += 500;
        }
    }
}