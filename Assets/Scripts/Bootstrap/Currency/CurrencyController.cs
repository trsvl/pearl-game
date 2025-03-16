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

            _model.OnCurrencyChanged += _view.UpdateCurrencyText;
        }
    }
}