using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MainMenu.UI.Header;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Bootstrap.Currency
{
    public class CurrencyView : IDisposable
    {
        private readonly CurrencyConverter _currencyConverter;
        private readonly StringBuilder _stringBuilder;

        private readonly Dictionary<CurrencyType, (RectTransform, TextMeshProUGUI, GameObject)> _currencyViews = new();
        private CancellationToken _cancellationToken;

        private GameObject _goldIconPrefab;
        private RectTransform _goldIcon;
        private TextMeshProUGUI _goldText;

        private GameObject __diamondIconPrefab;
        private RectTransform _diamondIcon;
        private TextMeshProUGUI _diamondText;


        public CurrencyView(CurrencyConverter currencyConverter, StringBuilder stringBuilder)
        {
            _currencyConverter = currencyConverter;
            _stringBuilder = stringBuilder;
        }

        public void InitHeader(MainMenuHeaderManager mainMenuHeaderManager, CancellationToken cancellationToken)
        {
            Debug.Log("INIT HEADER CURRENCY VIEW");

            _cancellationToken = cancellationToken;

            var header = mainMenuHeaderManager.GetHeader();

            _goldIcon = header._goldIcon;
            _goldText = header._goldText;

            _currencyViews.Add(CurrencyType.Gold, (_goldIcon, _goldText, _goldIconPrefab));

            _diamondIcon = header._diamondIcon;
            _diamondText = header._diamondText;

            _currencyViews.Add(CurrencyType.Diamond, (_diamondIcon, _diamondText, __diamondIconPrefab));
        }

        public void UpdateCurrencyText(CurrencyType type, ulong currencyValue, ulong addedValue)
        {
            _stringBuilder.Clear();
            bool isAddedValuePositive = addedValue > 0;
            ulong value = isAddedValuePositive ? currencyValue - addedValue : currencyValue;
            _stringBuilder.Append(_currencyConverter.Convert(value));
            _currencyViews[type].Item2.SetText(_stringBuilder);

            if (isAddedValuePositive) CollectCurrency(type, value, addedValue).Forget();
        }

        private async UniTask CollectCurrency(CurrencyType type, ulong startValue, ulong endValue)
        {
            int objectCount = Random.Range(5, 8);

            float objMoveDuration = 1f;

            for (int i = 0; i < objectCount; i++)
            {
                float randomX = Random.Range(1f, 3f);
                float randomY = Random.Range(1f, 3f);
                Vector2 randomSpawnPoint = new Vector2(randomX, 0 + randomY);
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

                GameObject obj = Object.Instantiate(_currencyViews[type].Item3, randomSpawnPoint, randomRotation);
                Vector3 endScale = obj.transform.localScale;
                obj.transform.localScale = Vector3.zero;

                await obj.transform.DOScale(endScale, 0.1f).SetUpdate(true)
                    .ToUniTask(cancellationToken: _cancellationToken);

                UniTask moveTask = obj.transform.DOMove(_currencyViews[type].Item1.position, objMoveDuration)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: _cancellationToken);

                moveTask.ContinueWith(() => Object.Destroy(obj.gameObject));
            }

            await UniTask.WaitForSeconds(objMoveDuration, cancellationToken: _cancellationToken);

            float duration = objMoveDuration * (objectCount - 2);
            UpdateCurrencyTextAnimation(type, startValue, endValue, duration);
        }

        private void UpdateCurrencyTextAnimation(CurrencyType type, ulong startValue, ulong endValue, float duration)
        {
            ulong value = startValue;
            Debug.Log("TEXT");
            DOTween.To(
                    () => value,
                    x => value = x,
                    endValue,
                    duration
                ).SetUpdate(true).OnUpdate(() =>
                {
                    _stringBuilder.Clear();
                    _stringBuilder.Append(_currencyConverter.Convert(value));
                    _currencyViews[type].Item2.SetText(_stringBuilder);
                })
                .ToUniTask(cancellationToken: _cancellationToken);
        }

        public void Dispose()
        {
            Debug.Log("CLEAR ALL THE CURRENCY VIEW");
            _currencyViews.Clear();
        }
    }
}