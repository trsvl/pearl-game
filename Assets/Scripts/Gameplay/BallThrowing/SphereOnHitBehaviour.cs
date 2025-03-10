using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Random;

namespace Gameplay.BallThrowing
{
    public class SphereOnHitBehaviour : IDestroySphere
    {
        private const int fallDuration = 5000;


        private void ChangeSphere(GameObject sphere)
        {
            sphere.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (sphere.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                float randomX = Range(0f, 0.5f);
                float randomZ = Range(0f, 0.5f);
                Vector3 randomPos = new Vector3(randomX, 1f, randomZ);
                
                rb.AddForce(randomPos, ForceMode.Impulse);
            }

            DestructAfterDelay(sphere);
        }

        private async UniTask DestructAfterDelay(GameObject sphere)
        {
            await UniTask.Delay(fallDuration);
            Object.Destroy(sphere);
        }

        public void OnDestroySphere(GameObject sphere)
        {
            ChangeSphere(sphere);
        }
    }
}