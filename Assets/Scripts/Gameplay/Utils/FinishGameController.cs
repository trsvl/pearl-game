using Bootstrap.Currency;
using Cysharp.Threading.Tasks;
using Gameplay.Animations;

namespace Gameplay.Utils
{
    public class FinishGameController : IFinishGame
    {
        private readonly ChangeHeader _changeHeader;
        private readonly CurrencyController _currencyController;

        
        public FinishGameController(ChangeHeader changeHeader, CurrencyController currencyController)
        {
            _changeHeader = changeHeader;
            _currencyController = currencyController;
        }

        public void FinishGame()
        {
            _changeHeader.Do(0.5f).Forget();
            _currencyController.UpdateCurrency(CurrencyType.Gold, 500);
            _currencyController.UpdateCurrency(CurrencyType.Diamond, 0);
        }
    }
}