using System.Collections;
using System.Collections.Generic;
using Gameplay.Header;
using UnityEngine;
using Utils.SphereData;

namespace Gameplay.BallThrowing
{
    public class SphereDestroyer : MonoBehaviour
    {
        public LayerMask layerMask;
        public float detectionRadius = 1f;
        public int maxSphereChecks = 10;
        public float wavePropagationDelay = 0.01f;
        public float fallDuration = 0.5f;
        public float pulseIntensity = 2f;
        public float pulseDuration = 0.3f;

        private Collider[] nearbySpheres;
        private PearlsData _pearlsData;
        private BallThrower _ballThrower;


        public void Init(PearlsData pearlsData)
        {
            _pearlsData = pearlsData;
            nearbySpheres = new Collider[maxSphereChecks];
        }

        public void TryInitiateWave(Collision targetCollision)
        {
            GameObject originSphere = targetCollision.gameObject;
            Color targetColor = originSphere.GetComponent<Renderer>().material.GetColor(AllColors.BaseColor);
            StartCoroutine(PropagateWave(originSphere, targetColor));
        }

        private IEnumerator PropagateWave(GameObject startSphere, Color targetColor = default)
        {
            Queue<GameObject> waveQueue = new Queue<GameObject>();
            HashSet<GameObject> processed = new HashSet<GameObject>();

            waveQueue.Enqueue(startSphere);
            processed.Add(startSphere);

            while (waveQueue.Count > 0)
            {
                int currentWaveSize = waveQueue.Count;

                for (int i = 0; i < currentWaveSize; i++)
                {
                    GameObject currentSphere = waveQueue.Dequeue();

                    if (!currentSphere) continue;

                    StartCoroutine(ProcessSphereDestruction(currentSphere));

                    int neighborsFound = Physics.OverlapSphereNonAlloc(
                        currentSphere.transform.position,
                        detectionRadius,
                        nearbySpheres,
                        layerMask
                    );

                    for (int j = 0; j < neighborsFound; j++)
                    {
                        Collider neighborCollider = nearbySpheres[j];
                        if (!neighborCollider) continue;

                        GameObject neighbor = neighborCollider.gameObject;
                        if (processed.Contains(neighbor)) continue;

                        Renderer neighborRenderer = neighbor.GetComponent<Renderer>();
                        if (!neighborRenderer) continue;

                        if (IsColorCompatible(neighborRenderer.material.GetColor(AllColors.BaseColor), targetColor))
                        {
                            processed.Add(neighbor);
                            waveQueue.Enqueue(neighbor);
                        }
                    }
                }

                yield return new WaitForSeconds(wavePropagationDelay);
            }
        }

        private bool IsColorCompatible(Color a, Color b, float threshold = 0.1f)
        {
            return Mathf.Abs(a.r - b.r) < threshold &&
                   Mathf.Abs(a.g - b.g) < threshold &&
                   Mathf.Abs(a.b - b.b) < threshold;
        }

        private IEnumerator ProcessSphereDestruction(GameObject sphere)
        {
            sphere.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (sphere.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            yield return new WaitForSeconds(fallDuration);

            _pearlsData.Count += 1;

            Destroy(sphere);
        }
    }
}