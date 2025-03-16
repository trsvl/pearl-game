using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Bootstrap.Currency
{
    public class CurrencyAnimation
    {
        private readonly Action<float> _onCollectCurrency;

        public async UniTask CollectCurrency(GameObject currencyIconPrefab, Vector3 targetPosition,
            CancellationToken cancellationToken, Action<float> action)
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
                    .ToUniTask(cancellationToken: cancellationToken);

                UniTask moveTask = obj.transform.DOMove(targetPosition, objMoveDuration)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: cancellationToken);

                moveTask.ContinueWith(() => Object.Destroy(obj.gameObject));
            }

            await UniTask.WaitForSeconds(objMoveDuration, cancellationToken: cancellationToken);

            float duration = objMoveDuration * (objectCount - 2);
            _onCollectCurrency?.Invoke(duration);
        }
    }
}