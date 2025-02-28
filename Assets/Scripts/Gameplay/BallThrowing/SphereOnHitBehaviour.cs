using System.Collections;
using Gameplay.Header;
using UnityEngine;

namespace Gameplay.BallThrowing
{
    public class SphereOnHitBehaviour : MonoBehaviour
    {
        private const float fallDuration = 5f;


        public void Init(PearlsData pearlsData)
        {
            _pearlsData = pearlsData;
        }

        public void DestroySphere(GameObject sphere)
        {
            sphere.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (sphere.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                
                Vector3 direction = sphere.transform.position - sphere.transform.parent.transform.position;
                rb.AddForce(direction.normalized * 2f, ForceMode.Impulse);
            }

            _pearlsData.Count += 1;

            StartCoroutine(DestructAfterDelay(sphere));
        }

        private IEnumerator DestructAfterDelay(GameObject sphere)
        {
            yield return new WaitForSeconds(fallDuration);
            Destroy(sphere);
        }
    }
}