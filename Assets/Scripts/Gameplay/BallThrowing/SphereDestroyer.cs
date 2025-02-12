using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BallThrowing
{
    public class SphereDestroyer : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        [Header("Raycast Settings")] public float maxRayDistance = 100f;
        public LayerMask sphereLayer;
        public LayerMask ignoreRaycastLayer;

        [Header("Wave Settings")] public float detectionRadius = 1f;
        public int maxSphereChecks = 10;
        public float wavePropagationDelay = 0.01f;
        public float fallDuration = 0.5f;

        [Header("Visual Feedback")] public float pulseIntensity = 2f;
        public float pulseDuration = 0.3f;

        private Collider[] nearbySpheres;


        private void Start()
        {
            nearbySpheres = new Collider[maxSphereChecks];
        }

        public void TryInitiateWave(Collision targetCollision)
        {
            GameObject originSphere = targetCollision.gameObject;
            Color targetColor = originSphere.GetComponent<Renderer>().material.GetColor(BaseColor);
            StartCoroutine(PropagateWave(originSphere, targetColor));
        }

        private IEnumerator PropagateWave(GameObject startSphere, Color targetColor)
        {
            Queue<GameObject> waveQueue = new Queue<GameObject>();
            HashSet<GameObject> processed = new HashSet<GameObject>();

            waveQueue.Enqueue(startSphere);
            processed.Add(startSphere);

            while (waveQueue.Count > 0)
            {
                GameObject currentSphere = waveQueue.Dequeue();

                StartCoroutine(VisualPulse(currentSphere));
                StartCoroutine(ProcessSphereDestruction(currentSphere));

                int neighborsFound = Physics.OverlapSphereNonAlloc(
                    currentSphere.transform.position,
                    detectionRadius,
                    nearbySpheres,
                    sphereLayer
                );

                for (int i = 0; i < neighborsFound; i++)
                {
                    if (!nearbySpheres[i]) continue;

                    GameObject neighbor = nearbySpheres[i].gameObject;
                    if (processed.Contains(neighbor)) continue;

                    Renderer neighborRenderer = neighbor.GetComponent<Renderer>();
                    if (!neighborRenderer) continue;

                    if (ColorMatch(neighborRenderer.material.GetColor(BaseColor), targetColor))
                    {
                        processed.Add(neighbor);
                        waveQueue.Enqueue(neighbor);
                        yield return new WaitForSeconds(wavePropagationDelay);
                    }
                }
            }
        }

        private IEnumerator VisualPulse(GameObject sphere)
        {
            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            Material mat = sphereRenderer.material;
            Color originalColor = mat.GetColor(BaseColor);

            float elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                mat.SetColor(BaseColor, Color.Lerp(originalColor * pulseIntensity, originalColor, elapsed / pulseDuration));
                elapsed += Time.deltaTime;
                yield return null;
            }

            mat.SetColor(BaseColor, originalColor);
        }

        private IEnumerator ProcessSphereDestruction(GameObject sphere)
        {
            sphere.layer = ignoreRaycastLayer;
            if (sphere.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            yield return new WaitForSeconds(fallDuration);
            Destroy(sphere);
        }

        private bool ColorMatch(Color a, Color b, float threshold = 0.01f)
        {
            return Vector4.Distance(a, b) < threshold;
        }
    }
}