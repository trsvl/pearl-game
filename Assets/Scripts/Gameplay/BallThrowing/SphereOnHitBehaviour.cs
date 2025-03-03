using System.Threading.Tasks;
using UnityEngine;
using Utils.EventBusSystem;

namespace Gameplay.BallThrowing
{
    public class SphereOnHitBehaviour : IDestroySphere
    {
        private const int fallDuration = 5000;


        public SphereOnHitBehaviour(EventBus eventBus)
        {
            eventBus.Subscribe(this);
        }

        private void ChangeSphere(GameObject sphere)
        {
            sphere.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (sphere.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                Vector3 direction = sphere.transform.position - sphere.transform.parent.transform.position;
                rb.AddForce(direction.normalized * 2f, ForceMode.Impulse);
            }

            DestructAfterDelay(sphere);
        }

        private async void DestructAfterDelay(GameObject sphere)
        {
            await Task.Delay(fallDuration);
            Object.Destroy(sphere);
        }

        public void OnDestroySphere(GameObject sphere)
        {
            ChangeSphere(sphere);
        }
    }
}