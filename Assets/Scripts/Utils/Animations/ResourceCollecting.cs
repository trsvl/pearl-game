using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Utils.Animations
{
    public class ResourceCollecting : IFinishGame
    {
        private readonly GameObject _coinPrefab;
        private readonly CancellationToken _cancellationToken;


        public ResourceCollecting(GameObject coinPrefab, CancellationToken cancellationToken)
        {
            _coinPrefab = coinPrefab;
            _cancellationToken = cancellationToken;
        }

        public void FinishGame()
        {
        }

        private async UniTask CollectCoinFinishGame(Transform spawnPoint, Transform targetPoint)
        {
            GameObject[] coins = new GameObject[20];
            
            for (int i = 0; i < coins.Length; i++)
            {
                Vector2 randomPoint = new Vector2(
                    spawnPoint.position.x + Random.value, spawnPoint.position.y + Random.value);
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                GameObject coin = Object.Instantiate(_coinPrefab, randomPoint, randomRotation);
                coin.transform.localScale = Vector3.zero;
                coins[i] = coin;

                await coin.transform.DOScale(1f, 0.05f).SetEase(Ease.Linear)
                    .ToUniTask(cancellationToken: _cancellationToken);
            }

            foreach (GameObject coin in coins)
            {
                UniTask moveTask = coin.transform.DOMove(targetPoint.position, 1f).SetEase(Ease.OutBounce)
                    .ToUniTask(cancellationToken: _cancellationToken);

                moveTask.ContinueWith(() => Object.Destroy(coin.gameObject));
            }
        }
    }
}