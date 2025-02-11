using System.Collections;
using UnityEngine;

namespace BallThrowing
{
    public class BallThrower : MonoBehaviour
    {
        [SerializeField] private SphereDestroyer sphereDestroyer;

        public GameObject ballPrefab;
        public Transform launchPoint;
        public float forceMultiplier = 0.3f;
        public float maxForce = 50f;
        public float verticalMultiplier = 1.5f;
        public float maxDragDistance = 300f;
        public LineRenderer lineRenderer;
        public int trajectoryPoints = 40;
        public float timeStep = 0.07f;
        public float respawnDelay = 0.3f;
        public float verticalForce = 0.5f;

        private Ball currentBall;
        private bool isDragging;
        private Vector3 initialMousePosition;


        private void Start() => SpawnBall();

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

            Vector3 throwDir = new Vector3(-screenDelta.x, screenDelta.magnitude * verticalMultiplier, -screenDelta.y)
                .normalized;

            UpdateTrajectoryLine(launchPoint.position, throwDir * forceMagnitude);
        }

        private void ReleaseBall()
        {
            isDragging = false;

            currentBall.InvokeDestroy();

            Vector3 currentMousePos = Input.mousePosition;
            Vector3 dragVector = currentMousePos - initialMousePosition;
            float dragMagnitude = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);
            float forceMagnitude = Mathf.Min(dragMagnitude * forceMultiplier, maxForce);

            Vector3 launchScreenPos = Camera.main.WorldToScreenPoint(launchPoint.position);
            Vector3 screenDelta = currentMousePos - launchScreenPos;

            Vector3 throwDir = new Vector3(-screenDelta.x, screenDelta.magnitude * verticalMultiplier, -screenDelta.y)
                .normalized;

            ApplyThrowForce(throwDir * forceMagnitude);
            lineRenderer.positionCount = 0;
        }

        private void ApplyThrowForce(Vector3 force)
        {
            if (!currentBall) return;

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
            currentBall = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity).AddComponent<Ball>();
            currentBall.transform.localScale = Vector3.one * 0.7f;
            currentBall.tag = "Untagged";
            currentBall.Init(sphereDestroyer);

            if (currentBall.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.drag = 0f;
            }

            lineRenderer.positionCount = 0;
        }

        private void UpdateTrajectoryLine(Vector3 startPos, Vector3 initialVelocity)
        {
            if (!lineRenderer) return;

            Vector3[] points = new Vector3[trajectoryPoints];
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float t = i * timeStep;
                points[i] = startPos + initialVelocity * t + Physics.gravity * (0.5f * t * t);
            }

            lineRenderer.positionCount = trajectoryPoints;
            lineRenderer.SetPositions(points);
        }

        private IEnumerator SpawnBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnBall();
        }
    }
}