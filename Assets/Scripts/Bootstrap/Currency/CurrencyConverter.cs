#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bootstrap.Currency
{
    public class CurrencyConverter
    {
        private readonly Dictionary<int, string> abbreviations = new()
        {
            { 3, "k" },
            { 6, "m" },
            { 9, "b" },
            { 12, "t" },
            { 15, "q" },
            { 18, "Q" },
        };


        public string Convert(ulong currency)
        {
            string currencyString = currency.ToString();

            int length = currencyString.Length;
            
            if (length <= 3) return currencyString;

            int exponent = length - (length % 3);

            if (!abbreviations.TryGetValue(exponent, out string abbreviation)) return currencyString;

            int mod = length % 3;
            int integerPartLength = mod == 0 ? 1 : mod;
            int decimalPartLength = 3 - integerPartLength;

            Span<char> buffer = stackalloc char[32];
            int index = 0;

            for (int i = 0; i < integerPartLength; i++)
            {
                buffer[index++] = currencyString[i];
            }

            if (decimalPartLength > 0)
            {
                buffer[index++] = '.';
                for (int i = 0; i < decimalPartLength; i++)
                {
                    buffer[index++] = currencyString[integerPartLength + i];
                }
            }

            foreach (char c in abbreviation)
            {
                buffer[index++] = c;
            }

            return new string(buffer.Slice(0, index));
        }
    }
}