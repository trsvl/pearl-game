using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bootstrap.Currency
{
    public class CurrencyModel : IDisposable
    {
        public event Action<CurrencyType, ulong, ulong> OnCurrencyChanged;

        private readonly Dictionary<CurrencyType, ulong> _currencies = new();
        private const string GOLD_CURRENCY = "GoldCurrency";
        private const string DIAMOND_CURRENCY = "DiamondCurrency";


        public CurrencyModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var goldCurrencyValue = PlayerPrefs.GetString(GOLD_CURRENCY, "550");
            var goldCurrency = ParseData(goldCurrencyValue);
            _currencies.Add(CurrencyType.Gold, goldCurrency);

            var diamondValue = PlayerPrefs.GetString(DIAMOND_CURRENCY, "1560");
            var diamondCurrency = ParseData(diamondValue);
            _currencies.Add(CurrencyType.Diamond, diamondCurrency);
        }

        public void UpdateCurrency(CurrencyType type, ulong value)
        {
            if (value > 0) AddCurrency(type, value);
            else RemoveCurrency(type, value);
        }

        private void AddCurrency(CurrencyType type, ulong value)
        {
            if (!_currencies.TryGetValue(type, out ulong currency)) return;

            if (currency > ulong.MaxValue - value)
            {
                currency = ulong.MaxValue;
            }
            else currency += value;

            OnCurrencyChanged?.Invoke(type, currency, value);
        }

        private void RemoveCurrency(CurrencyType type, ulong value)
        {
            if (!_currencies.TryGetValue(type, out ulong currency)) return;

            if (currency < ulong.MinValue + value)
            {
                currency = ulong.MinValue;
            }
            else currency -= value;

            OnCurrencyChanged?.Invoke(type, currency, value);
        }

        private ulong ParseData(string data)
        {
            return ulong.TryParse(data, out var result) ? result : 0;
        }

        public void Dispose()
        {
            _currencies.Clear();
        }
    }
}