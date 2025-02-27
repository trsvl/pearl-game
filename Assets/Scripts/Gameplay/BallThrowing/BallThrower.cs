using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Header;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.SphereData;

namespace Gameplay.BallThrowing
{
    public class BallThrower : MonoBehaviour, IStartGame, ILoseGame, IPauseGame, IResumeGame, IFinishGame
    {
        [SerializeField] private SphereDestroyer sphereDestroyer;
        [SerializeField] private float dragSensitivity = 0.1f;
        [SerializeField] private float verticalMultiplier = 3f;
        [SerializeField] private int trajectoryPoints = 40;
        [SerializeField] private float timeStep = 0.07f;
        [SerializeField] private float respawnDelay = 0.3f;
        [SerializeField] private LayerMask collisionMaskLine;
        [SerializeField] private RectTransform dragArea;
        [SerializeField] private Camera uiCamera;

        private Camera _mainCamera;
        private GameObject _ballPrefab;
        private Vector3 _ballSpawnPoint;
        private Color _previousColor = new(0, 0, 0, 0);
        private LineRenderer _lineRenderer;
        private Ball _currentBall;
        private bool _isDragging;
        private Vector3 _initialMousePosition;
        private Color[] _levelColors;
        private ShotsData _shotData;
        private bool _isAllowedToDrag;
        private Vector3 _velocity;

        private float _ballSize;
        private Vector3 _ballLocalScale;

        private const float _minimalForce = 3f;
        private const float _maxForce = 50f;


        public void Init(Color[] levelColors, ShotsData shotData, SpheresDictionary spheresDictionary)
        {
            _ballPrefab = Resources.Load<GameObject>("Prefabs/ThrowingBall");

            _levelColors = levelColors;
            _shotData = shotData;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;

            Vector3 lowestScale = spheresDictionary.GetLowestSphereScale();
            InitBallData(lowestScale);
            InitCameraData();
        }

        private void Update()
        {
            if (!_isAllowedToDrag || !_currentBall) return;

            HandleInput();

            if (_isDragging)
            {
                if (!IsMouseInDragArea())
                {
                    _isDragging = false;
                    _lineRenderer.positionCount = 0;
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
                if (IsPointerOverUIElement()) return;

                _isDragging = true;
                _initialMousePosition = Input.mousePosition;
                _lineRenderer.positionCount = trajectoryPoints;

                Color color = _currentBall._renderer.material.GetColor(AllColors.BaseColor);
                _lineRenderer.startColor = color;
                _lineRenderer.endColor = color;
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                _isDragging = false;
                _lineRenderer.positionCount = 0;

                ReleaseBall();
                StartCoroutine(SpawnBallAfterDelay(respawnDelay));
            }
        }

        private void UpdateTrajectory()
        {
            Vector2 dragDelta = (Input.mousePosition - _initialMousePosition) * dragSensitivity;

            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;
            Vector3 cameraUp = _mainCamera.transform.up;

            Vector3 throwDir = (
                cameraForward * 100f +
                cameraRight * (dragDelta.x * 0.2f) +
                cameraUp * (dragDelta.y * verticalMultiplier * 0.2f)
            ).normalized;

            float dragMagnitude = Mathf.Clamp(dragDelta.magnitude, 0, 200f);
            float forceMagnitude = Mathf.Max(_minimalForce, Mathf.Min(dragMagnitude, _maxForce));
            _velocity = throwDir * forceMagnitude;

            UpdateTrajectoryLine(_ballSpawnPoint, _velocity);
        }

        private void UpdateTrajectoryLine(Vector3 startPos, Vector3 initialVelocity)
        {
            if (!_lineRenderer) return;

            var points = new Vector3[trajectoryPoints];
            points[0] = startPos;
            int actualPoints = trajectoryPoints;

            for (var i = 1; i < trajectoryPoints; i++)
            {
                float time = i * timeStep;

                points[i] = CalculatePositionAtTime(startPos, initialVelocity, time, Physics.gravity);

                if (Physics.Raycast(points[i - 1], (points[i] - points[i - 1]).normalized, out RaycastHit hit,
                        Vector3.Distance(points[i - 1], points[i]), collisionMaskLine))
                {
                    points[i] = hit.point;
                    actualPoints = i + 1;
                    break;
                }
            }

            _lineRenderer.positionCount = actualPoints;
            _lineRenderer.SetPositions(points);
        }

        private Vector3 CalculatePositionAtTime(Vector3 startPos, Vector3 startVelocity, float time, Vector3 gravity)
        {
            return startPos + startVelocity * time + gravity * (0.5f * time * time);
        }

        private void ReleaseBall()
        {
            _isDragging = false;
            _lineRenderer.positionCount = 0;
            _shotData.Count -= 1;

            if (!_currentBall) return;
            StartCoroutine(DestroyBallAfterDelay(_currentBall.gameObject));

            ApplyThrowForce(_velocity);
        }

        private void ApplyThrowForce(Vector3 force)
        {
            var rb = _currentBall.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.VelocityChange);
            _currentBall = null;
        }

        private void SpawnBall()
        {
            if (_shotData.Count <= 0) return;

            _currentBall = Instantiate(_ballPrefab, _ballSpawnPoint, Quaternion.identity).AddComponent<Ball>();
            _currentBall.Init(sphereDestroyer);

            _currentBall.transform.localScale = _ballLocalScale;
            _currentBall._renderer.material.SetColor(AllColors.BaseColor, GetBallColor());
        }

        private void InitBallData(Vector3 ballLocalScale)
        {
            _ballLocalScale = ballLocalScale;

            var initialBall = Instantiate(_ballPrefab).AddComponent<Ball>();
            initialBall.transform.localScale = _ballLocalScale;
            initialBall.gameObject.SetActive(false);
            Renderer ballRenderer = initialBall.GetComponent<Renderer>();
            Bounds bounds = ballRenderer.bounds;
            const float ballScale = 0.3f;
            Vector3 objectSizes = (bounds.max - bounds.min) / ballScale;

            _ballSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);

            Destroy(initialBall.gameObject);
        }

        private void InitCameraData()
        {
            _mainCamera = Camera.main;
            UpdateCameraFOV();
        }

        private void UpdateCameraFOV()
        {
            float cameraView =
                2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _mainCamera.fieldOfView);
            float distance = 2f * _ballSize / cameraView;
            distance += 0.5f * _ballSize;
            _mainCamera.transform.position = new Vector3(0, 0, distance);

            Vector3 ballPosition = new Vector3(0.8f, 0.15f, distance);
            _ballSpawnPoint = _mainCamera.ViewportToWorldPoint(ballPosition);

            if (!_currentBall) return;

            _currentBall.transform.position = _ballSpawnPoint;
        }

        public void RespawnBall()
        {
            if (!_isAllowedToDrag) return;
            
            if (_currentBall != null) Destroy(_currentBall.gameObject);

            UpdateCameraFOV();
            SpawnBall();
        }

        private Color GetBallColor()
        {
            if (_levelColors.Length <= 1) return _levelColors[0];
            Color[] filteredColors = _levelColors.Where(color => color != _previousColor).ToArray();
            int randomIndex = Random.Range(0, filteredColors.Length);
            _previousColor = filteredColors[randomIndex];
            return filteredColors[randomIndex];
        }

        private IEnumerator SpawnBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnBall();
        }

        private bool IsMouseInDragArea()
        {
            Vector3 worldMousePosition = Input.mousePosition;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(dragArea, worldMousePosition, uiCamera,
                out Vector3 worldPoint);

            Vector2 localMousePosition = dragArea.InverseTransformPoint(worldPoint);

            return dragArea.rect.Contains(localMousePosition);
        }

        private bool IsPointerOverUIElement()
        {
            if (!EventSystem.current) return false;
            
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        private IEnumerator DestroyBallAfterDelay(GameObject ball)
        {
            yield return new WaitForSeconds(4f);
            Destroy(ball);
        }

        public void StartGame()
        {
            SpawnBall();
            _isAllowedToDrag = true;
        }

        public void LoseGame()
        {
            _isAllowedToDrag = false;
        }

        public void PauseGame()
        {
            _isAllowedToDrag = false;
        }

        public void ResumeGame()
        {
            _isAllowedToDrag = true;
        }

        public void FinishGame()
        {
            _isAllowedToDrag = false;
        }
    }
}