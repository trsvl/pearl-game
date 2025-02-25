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
        private AllSpheresData _allSpheresData;


        public void Init(PearlsData pearlsData, AllSpheresData allSpheresData)
        {
            _pearlsData = pearlsData;
            _allSpheresData = allSpheresData;
        }

        public void DestroySpheresSegment(Collision targetCollision, Color targetColor)
        {
            GameObject targetSphere = targetCollision.gameObject;
            StartCoroutine(_allSpheresData.DestroySpheresSegment(targetColor, targetSphere, DestroySphere));
        }

        private void DestroySphere(GameObject sphere)
        {
            sphere.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (sphere.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
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