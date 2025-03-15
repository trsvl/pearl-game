using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Animations;
using TMPro;
using UnityEngine;

namespace Bootstrap
{
    public class CurrencyAnimation
    {
        private readonly CurrencyConverter _currencyConverter;
        private readonly CameraManager _cameraManager;
        private readonly CancellationToken _cancellationToken;
        private readonly StringBuilder _stringBuilder;


        public CurrencyAnimation(CurrencyConverter currencyConverter, CameraManager cameraManager,
            CancellationToken cancellationToken)
        {
            _currencyConverter = currencyConverter;
            _cameraManager = cameraManager;
            _cancellationToken = cancellationToken;
            _stringBuilder = new StringBuilder();
        }

        public async UniTask Collect(GameObject currencyIconPrefab, Vector3 targetPosition,
            TextMeshProUGUI currencyText, ulong currencyValue,
            ulong currency)
        {
            int objectCount = Random.Range(5, 8);

            float objMoveDuration = 1f;

            for (int i = 0; i < objectCount; i++)
            {
                float randomX = Random.Range(1f, 3f);
                float randomY = Random.Range(1f, 3f);
                Vector2 randomSpawnPoint = new Vector2(randomX, 0 + randomY);
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                GameObject obj = Object.Instantiate(currencyIconPrefab, randomSpawnPoint, randomRotation);
                Vector3 endScale = obj.transform.localScale;
                obj.transform.localScale = Vector3.zero;

                await obj.transform.DOScale(endScale, 0.1f).SetUpdate(true)
                    .ToUniTask(cancellationToken: _cancellationToken);

                UniTask moveTask = obj.transform.DOMove(targetPosition, objMoveDuration)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: _cancellationToken);

                moveTask.ContinueWith(() => Object.Destroy(obj.gameObject));
            }

            await UniTask.WaitForSeconds(objMoveDuration, cancellationToken: _cancellationToken);

            IncreaseCurrencyValueOverTime(currencyText, currencyValue, currency,
                (objectCount - 2) * objMoveDuration);
        }

        private void IncreaseCurrencyValueOverTime(TextMeshProUGUI currencyText, ulong startValue, ulong endValue,
            float duration)
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
                    _stringBuilder.Append($"{_currencyConverter.Convert(value)}");
                    currencyText.SetText(_stringBuilder);
                })
                .ToUniTask(cancellationToken: _cancellationToken);
        }
    }
}