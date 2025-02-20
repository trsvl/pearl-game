using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Header;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.SphereData;

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
        public GameObject throwAreaObject;

        private Color previousColor = new(0, 0, 0, 0);
        private Bounds dragAreaBounds;
        private LineRenderer lineRenderer;
        private Ball currentBall;
        private bool isDragging;
        private Vector3 initialMousePosition;
        private Color[] _levelColors;
        private ShotsData _shotData;
        private GameplayStateObserver _gameplayStateObserver;


        public void Init(Color[] levelColors, ShotsData shotData, GameplayStateObserver gameplayStateObserver)
        {
            _levelColors = levelColors;
            _shotData = shotData;
            _gameplayStateObserver = gameplayStateObserver;
            lineRenderer = GetComponent<LineRenderer>();
            dragAreaBounds = throwAreaObject.GetComponent<Collider>().bounds;

            SpawnBall();
        }

        private void Update()
        {
            if (!currentBall) return;
            if (_gameplayStateObserver.GameplayState != GameplayState.PLAY) return;

            HandleInput();

            if (isDragging)
            {
                if (!IsMouseInDragArea())
                {
                    isDragging = false;
                    lineRenderer.positionCount = 0;
                }
                else
                {
                    UpdateTrajectory();
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsMouseInDragArea()) return;

                isDragging = true;
                initialMousePosition = Input.mousePosition;
                lineRenderer.positionCount = trajectoryPoints;
                lineRenderer.startColor = currentBall._renderer.material.GetColor(AllColors.BaseColor);
                lineRenderer.endColor = currentBall._renderer.material.GetColor(AllColors.BaseColor);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                if (IsPointerOverUIElement())
                {
                    isDragging = false;
                    lineRenderer.positionCount = 0;
                }
                else
                {
                    ReleaseBall();
                    StartCoroutine(SpawnBallAfterDelay(respawnDelay));
                }
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

            Vector3 currentVelocity = new Vector3(initialVelocity.x, initialVelocity.y * 1.2f, initialVelocity.z);

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
            currentBall.GetComponent<Renderer>().material.SetColor(AllColors.BaseColor, GetBallColor());

            currentBall.Init(sphereDestroyer, LayerMask.NameToLayer("Ignore Raycast"));
        }

        public void RespawnBall()
        {
            if (currentBall != null) Destroy(currentBall.gameObject);

            SpawnBall();
        }

        private Color GetBallColor()
        {
            if (_levelColors.Length <= 1) return _levelColors[0];

            Color[] filteredColors = _levelColors.Where(color => !IsEqualTo(color, previousColor)).ToArray();
            int randomIndex = Random.Range(0, filteredColors.Length);
            previousColor = filteredColors[randomIndex];

            return filteredColors[randomIndex];
        }

        private IEnumerator SpawnBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnBall();
        }

        private bool IsMouseInDragArea()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("DragArea")))
            {
                return dragAreaBounds.Contains(hit.point);
            }

            return false;
        }

        private bool IsPointerOverUIElement()
        {
            if (!EventSystem.current) return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        private bool IsEqualTo(Color a, Color b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }
    }
}