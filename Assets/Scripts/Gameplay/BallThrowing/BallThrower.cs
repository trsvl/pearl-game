using System.Collections;
using System.Linq;
using Gameplay.Header;
using UnityEngine;

namespace Gameplay.BallThrowing
{
    public class BallThrower : MonoBehaviour
    {
        [SerializeField] private SphereDestroyer sphereDestroyer;

        [Header("Settings")] public GameObject ballPrefab;
        public Transform launchPoint;
        public float forceMultiplier = 0.3f;
        public float maxForce = 50f;
        public float verticalMultiplier = 1.5f;
        public float maxDragDistance = 300f;
        public int trajectoryPoints = 40;
        public float timeStep = 0.07f;
        public float respawnDelay = 0.3f;
        public LayerMask collisionMaskLine;
        public LayerMask collisionMaskBall;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private Material previousMaterial;
        private LineRenderer lineRenderer;
        private Ball currentBall;
        private bool isDragging;
        private Vector3 initialMousePosition;

        private Material[] _materials;
        private ShotsData _shotData;

        public void Init(Material[] materials, ShotsData shotData)
        {
            _materials = materials;
            _shotData = shotData;
            lineRenderer = GetComponent<LineRenderer>();
            SpawnBall();
        }

        private void Update()
        {
            if (!currentBall) return;

            HandleInput();
            if (isDragging) UpdateTrajectory();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                initialMousePosition = Input.mousePosition;
                lineRenderer.positionCount = trajectoryPoints;
                lineRenderer.startColor = currentBall._renderer.material.GetColor(BaseColor);
                lineRenderer.endColor = currentBall._renderer.material.GetColor(BaseColor);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                ReleaseBall();
                StartCoroutine(SpawnBallAfterDelay(respawnDelay));
            }
        }

        private void UpdateTrajectory()
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 dragVector = currentMousePos - initialMousePosition;
            float dragMagnitude = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);
            float forceMagnitude = Mathf.Min(dragMagnitude * forceMultiplier, maxForce);

            Vector3 launchScreenPos = Camera.main.WorldToScreenPoint(launchPoint.position);
            Vector3 screenDelta = currentMousePos - launchScreenPos;

            Vector3 throwDir = new Vector3(
                -screenDelta.x,
                screenDelta.magnitude * verticalMultiplier,
                -screenDelta.y
            ).normalized;

            UpdateTrajectoryLine(launchPoint.position, throwDir * forceMagnitude);
        }

        private void UpdateTrajectoryLine(Vector3 startPos, Vector3 initialVelocity)
        {
            if (!lineRenderer) return;

            Vector3[] points = new Vector3[trajectoryPoints];
            points[0] = startPos;
            int actualPoints = trajectoryPoints;

            Vector3 currentPosition = startPos;
            Vector3 currentVelocity = initialVelocity;

            for (int i = 1; i < trajectoryPoints; i++)
            {
                currentVelocity += Physics.gravity * timeStep;
                Vector3 newPosition = currentPosition + currentVelocity * timeStep;

                if (Physics.Linecast(currentPosition, newPosition, out RaycastHit hit, collisionMaskLine))
                {
                    points[i] = hit.point;
                    actualPoints = i + 1;
                    break;
                }

                points[i] = newPosition;
                currentPosition = newPosition;
            }

            lineRenderer.positionCount = actualPoints;
            lineRenderer.SetPositions(points);
        }

        private void ReleaseBall()
        {
            isDragging = false;
            lineRenderer.positionCount = 0;
            _shotData.Count -= 1;

            if (!currentBall) return;

            currentBall.InvokeDestroy();
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 dragVector = currentMousePos - initialMousePosition;
            float dragMagnitude = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);
            float forceMagnitude = Mathf.Min(dragMagnitude * forceMultiplier, maxForce);

            Vector3 launchScreenPos = Camera.main.WorldToScreenPoint(launchPoint.position);
            Vector3 screenDelta = currentMousePos - launchScreenPos;

            Vector3 throwDir = new Vector3(
                -screenDelta.x,
                screenDelta.magnitude * verticalMultiplier,
                -screenDelta.y
            ).normalized;

            ApplyThrowForce(throwDir * forceMagnitude);
        }

        private void ApplyThrowForce(Vector3 force)
        {
            Rigidbody rb = currentBall.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.velocity = new Vector3(force.x, force.y * 1.2f, force.z);
            }

            currentBall = null;
        }

        private void SpawnBall()
        {
            if (_shotData.Count <= 0) return;

            currentBall = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity).AddComponent<Ball>();
            currentBall.GetComponent<Renderer>().material = GetBallMaterial();

            currentBall.Init(sphereDestroyer, collisionMaskBall);
        }

        private Material GetBallMaterial()
        {
            if (_materials.Length <= 1) return _materials[0];

            Material[] filteredMaterials = _materials.Where(material => material != previousMaterial).ToArray();
            int randomIndex = Random.Range(0, filteredMaterials.Length);

            return filteredMaterials[randomIndex];
        }

        private IEnumerator SpawnBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnBall();
        }
    }
}