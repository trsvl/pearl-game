using Bootstrap.Currency;
using Cysharp.Threading.Tasks;
using Gameplay.Animations;

namespace Gameplay.Utils
{
    public class FinishGameController
    {
        private readonly ChangeHeader _changeHeader;
        private readonly CurrencyController _currencyController;

        
        public FinishGameController(ChangeHeader changeHeader, CurrencyController currencyController)
        {
            _changeHeader = changeHeader;
            _currencyController = currencyController;
        }

        public async UniTask FinishGame()
        {
            await _changeHeader.Swap(0.25f);
            _currencyController.UpdateCurrency(CurrencyType.Gold, 500);
        }
    }
}