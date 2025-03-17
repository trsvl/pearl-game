using System.Threading;
using MainMenu.UI.Header;

namespace Bootstrap.Currency
{
    public class CurrencyController
    {
        private readonly CurrencyModel _model;
        private readonly CurrencyView _view;

        public CurrencyController(CurrencyModel model, CurrencyView view)
        {
            _model = model;
            _view = view;

            _model.OnCurrencyChanged += view.UpdateCurrencyText;
        }

        public void UpdateCurrency(CurrencyType type, ulong amount)
        {
            _model.UpdateCurrency(type, amount);
        }

        public void InitMainMenuHeaderCurrencies(MainMenuHeader header, CancellationToken cancellationToken)
        {
            _view.InitHeader(header, cancellationToken);
            UpdateCurrency(CurrencyType.Gold, 0);
            UpdateCurrency(CurrencyType.Diamond, 0);
        }

        public void ClearMainMenuHeaderCurrencies()
        {
            _view.ClearHeader();
        }
    }
}