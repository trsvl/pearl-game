using Bootstrap;
using Bootstrap.Currency;
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
        private readonly CurrencyModel _currencyModel;

        public FinishGameController(EventBus eventBus, MoveUIAnimation moveUIAnimation,
            GamePopupManager gamePopupManager, ChangeHeader changeHeader, CurrencyModel currencyModel)
        {
            _eventBus = eventBus;
            _moveUIAnimation = moveUIAnimation;
            _gamePopupManager = gamePopupManager;
            _changeHeader = changeHeader;
            _currencyModel = currencyModel;
        }

        public void FinishGame()
        {
            _changeHeader.Do(0.5f).Forget();
            _currencyModel.GoldCurrency += 500;
        }
    }
}