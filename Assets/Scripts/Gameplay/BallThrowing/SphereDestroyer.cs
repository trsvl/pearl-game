using System.Collections;
using Gameplay.Header;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay.BallThrowing
{
    public class SphereDestroyer : MonoBehaviour
    {
        private const float fallDuration = 5f;
        private PearlsData _pearlsData;
        private SpheresDictionary _spheresDictionary;


        public void Init(PearlsData pearlsData, SpheresDictionary spheresDictionary)
        {
            _pearlsData = pearlsData;
            _spheresDictionary = spheresDictionary;
        }

        public void DestroySpheresSegment(Collision targetCollision, Color targetColor)
        {
            GameObject targetSphere = targetCollision.gameObject;
            StartCoroutine(_spheresDictionary.DestroySpheresSegment(targetColor, targetSphere, DestroySphere));
        }

        private void DestroySphere(GameObject sphere)
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