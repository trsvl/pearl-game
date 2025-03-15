using Cysharp.Threading.Tasks;
using Gameplay.Animations;
using TMPro;
using UnityEngine;

namespace Bootstrap
{
    public class Currencies
    {
        public ulong GoldCurrency
        {
            get => _goldCurrency;
            set
            {
                if (value > 0) AddCurrency(_goldIconPrefab, _goldIconPosition, _goldCurrencyText, value, ref _goldCurrency);
                else RemoveCurrency(_goldCurrencyText, value, ref _goldCurrency);
            }
        }

        public ulong DiamondCurrency
        {
            get => _diamondCurrency;
            set
            {
                if (value > 0) AddCurrency(_diamondIconPrefab,_diamondIconPosition, _diamondCurrencyText, value, ref _diamondCurrency);
                else RemoveCurrency(_diamondCurrencyText, value, ref _diamondCurrency);
            }
        }

        private readonly GameObject _goldIconPrefab;
        private readonly GameObject _diamondIconPrefab;
        private readonly CurrencyAnimation _currencyAnimation;
        private readonly CurrencyConverter _currencyConverter;
        private readonly CameraManager _cameraManager;

        private const string GOLD_CURRENCY = "GoldCurrency";
        private const string DIAMOND_CURRENCY = "DiamondCurrency";
        private ulong _goldCurrency;
        private ulong _diamondCurrency;
        private TextMeshProUGUI _goldCurrencyText;
        private TextMeshProUGUI _diamondCurrencyText;
        private Vector3 _goldIconPosition;
        private Vector3 _diamondIconPosition;

        public Currencies(GameObject goldIconPrefab, GameObject diamondIconPrefab,
            CurrencyAnimation currencyAnimation, CurrencyConverter currencyConverter, CameraManager cameraManager)
        {
            _goldIconPrefab = goldIconPrefab;
            _diamondIconPrefab = diamondIconPrefab;
            _currencyAnimation = currencyAnimation;
            _currencyConverter = currencyConverter;
            _cameraManager = cameraManager;
            LoadData();
            Debug.Log(GOLD_CURRENCY);
        }

        public void LoadData()
        {
            var goldCurrencyValue = PlayerPrefs.GetString(GOLD_CURRENCY, "550");
            _goldCurrency = ParseData(goldCurrencyValue);

            var diamondValue = PlayerPrefs.GetString(DIAMOND_CURRENCY, "1560");
            _diamondCurrency = ParseData(diamondValue);
        }

        public void UpdateData(ulong gold, ulong diamond)
        {
            PlayerPrefs.SetString(GOLD_CURRENCY, GoldCurrency.ToString());
        }

        private void AddCurrency(GameObject iconPrefab, Vector3 targetPosition, TextMeshProUGUI textMeshProUGUI, ulong value,
            ref ulong currency)
        {
            if (currency > ulong.MaxValue - value)
            {
                currency = ulong.MaxValue;
            }
            else currency += value;

            _currencyAnimation.Collect(iconPrefab, targetPosition, textMeshProUGUI, value, currency).Forget();
        }

        private void RemoveCurrency(TextMeshProUGUI textMeshProUGUI, ulong value, ref ulong currency)
        {
            if (currency < ulong.MinValue + value)
            {
                currency = ulong.MinValue;
            }
            else currency -= value;

            textMeshProUGUI?.SetText(_currencyConverter.Convert(currency));
        }

        private void SetCurrency(TextMeshProUGUI textMeshProUGUI, ref ulong currency)
        {
            textMeshProUGUI?.SetText(_currencyConverter.Convert(currency));
        }

        public void BindGoldCurrencyText(TextMeshProUGUI textMeshProUGUI)
        {
            _goldCurrencyText = textMeshProUGUI;
            SetCurrency(_goldCurrencyText, ref _goldCurrency);
        }

        public void BindDiamondCurrencyText(TextMeshProUGUI textMeshProUGUI)
        {
            _diamondCurrencyText = textMeshProUGUI;
            SetCurrency(_diamondCurrencyText, ref _diamondCurrency);
        }
        
        public void BindGoldIcon(RectTransform rectTransform)
        {
            _goldIconPosition = rectTransform.position;
            Debug.Log("init icon pos:" + rectTransform.position);
            Debug.Log(_goldIconPosition);
        }

        public void BindDiamondIcon(RectTransform rectTransform)
        {
         
        }

        private ulong ParseData(string data)
        {
            return ulong.TryParse(data, out var result) ? result : 0;
        }
    }
}