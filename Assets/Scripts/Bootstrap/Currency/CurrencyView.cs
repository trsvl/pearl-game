using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MainMenu.UI.Header;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Bootstrap.Currency
{
    public class CurrencyView : IDisposable
    {
        private readonly GameObject _currencyPrefab;
        private readonly CurrencyConverter _currencyConverter;
        private readonly StringBuilder _stringBuilder;
        private readonly CameraController _cameraController;

        private readonly Dictionary<CurrencyType, (RectTransform, TextMeshProUGUI)> _currencyViews = new();
        private CancellationToken _cancellationToken;


        public CurrencyView(GameObject currencyPrefab, CurrencyConverter currencyConverter, StringBuilder stringBuilder,
            CameraController cameraController)
        {
            _currencyPrefab = currencyPrefab;
            _currencyConverter = currencyConverter;
            _stringBuilder = stringBuilder;
            _cameraController = cameraController;
        }

        public void InitHeader(MainMenuHeader header, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            _currencyViews.Add(CurrencyType.Gold, (header._goldIcon, header._goldText));
            _currencyViews.Add(CurrencyType.Diamond, (header._diamondIcon, header._diamondText));
        }

        public void ClearHeader()
        {
            if (_currencyViews.ContainsKey(CurrencyType.Gold))
            {
                _currencyViews.Remove(CurrencyType.Gold);
            }

            if (_currencyViews.ContainsKey(CurrencyType.Diamond))
            {
                _currencyViews.Remove(CurrencyType.Diamond);
            }
        }

        public void UpdateCurrencyText(CurrencyType type, ulong currencyValue, ulong addedValue)
        {
            _stringBuilder.Clear();
            bool isAddedValuePositive = addedValue > 0;
            ulong value = isAddedValuePositive ? currencyValue - addedValue : currencyValue;
            _stringBuilder.Append(_currencyConverter.Convert(value));
            _currencyViews[type].Item2.SetText(_stringBuilder);

            if (isAddedValuePositive) CollectCurrency(type, value, currencyValue).Forget();
        }

        private async UniTask CollectCurrency(CurrencyType type, ulong startValue, ulong endValue)
        {
            int objectCount = Random.Range(5, 8);
            float spawnDelay = 0.1f;
            float objMoveDuration = 1f;

            await UniTask.DelayFrame(1, cancellationToken: _cancellationToken);

            MoveObjects(type, objectCount, spawnDelay, objMoveDuration).Forget();

            await UniTask.WaitForSeconds(objMoveDuration + spawnDelay, cancellationToken: _cancellationToken,
                ignoreTimeScale: true);

            float textUpdateDuration = (spawnDelay) * (objectCount - 2) + 0.2f;
            UpdateCurrencyTextAnimation(type, startValue, endValue, textUpdateDuration);
        }

        private async UniTask MoveObjects(CurrencyType type, int objectCount, float spawnDelay, float objMoveDuration)
        {
            var prefab = Object.Instantiate(_currencyPrefab);
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            Sprite sprite = _currencyViews[type].Item1.GetComponent<Image>().sprite;
            spriteRenderer.sprite = sprite;
            Vector3 targetScale = CalculateObjectLocalScale(spriteRenderer, _currencyViews[type].Item1);
            prefab.transform.position = Vector3.one * 1000f;

            for (int i = 0; i < objectCount; i++)
            {
                float randomX = Random.Range(-1f, 1f);
                float randomY = Random.Range(-1f, 1f);

                Vector2 randomSpawnPoint = new Vector2(randomX, randomY);
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

                GameObject obj = Object.Instantiate(prefab, randomSpawnPoint, randomRotation);
                obj.transform.localScale = Vector3.zero;
                prefab.SetActive(true);

                await obj.transform.DOScale(targetScale, spawnDelay)
                    .SetUpdate(true)
                    .SetEase(Ease.OutBack)
                    .ToUniTask(cancellationToken: _cancellationToken);

                UniTask moveTask = obj.transform.DOMove(_currencyViews[type].Item1.position, objMoveDuration)
                    .SetUpdate(true)
                    .SetEase(Ease.InBack)
                    .ToUniTask(cancellationToken: _cancellationToken);

                moveTask.ContinueWith(() => Object.Destroy(obj.gameObject)).Forget();
            }

            Object.Destroy(prefab);
        }

        private Vector3 CalculateObjectLocalScale(SpriteRenderer spriteRenderer, RectTransform uiElement)
        {
            Vector2 spritePixelSize = spriteRenderer.sprite.rect.size;
            float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
            Vector2 spriteWorldSize = spritePixelSize / pixelsPerUnit;

            Vector2 uiSize = uiElement.rect.size;
            Vector3 uiWorldSize = uiElement.TransformVector(uiSize);

            Vector3 scaleFactor = new Vector3(
                uiWorldSize.x / spriteWorldSize.x,
                uiWorldSize.x / spriteWorldSize.x,
                uiWorldSize.x / spriteWorldSize.x
            );

            return scaleFactor;
        }

        private void UpdateCurrencyTextAnimation(CurrencyType type, ulong startValue, ulong endValue, float duration)
        {
            ulong value = startValue;
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
            _currencyViews.Clear();
        }
    }
}