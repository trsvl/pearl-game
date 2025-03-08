using System.Collections.Generic;
using Gameplay.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Gameplay.BallThrowing
{
    public class BallThrower : MonoBehaviour, IStartGame, ILoseGame, IPauseGame, IResumeGame, IFinishGame
    {
        [SerializeField] private float _dragSensitivity = 0.1f;
        [SerializeField] private float _verticalMultiplier = 3f;
        [SerializeField] private int _trajectoryPoints = 40;
        [SerializeField] private float _timeStep = 0.07f;
        [SerializeField] private LayerMask _collisionMaskLine;
        [SerializeField] private RectTransform _dragArea;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private float _minimalForce = 1f;
        [SerializeField] private float _maxForce = 30f;

        private BallFactory _ballFactory;
        private LineRenderer _lineRenderer;
        private Camera _mainCamera;
        private bool _isDragging;
        private bool _isAllowedToDrag;
        private Vector3 _initialMousePosition;
        private Vector3 _throwDirection;


        [Inject]
        public void Init(BallFactory ballFactory)
        {
            _ballFactory = ballFactory;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;

            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!_isAllowedToDrag || !_ballFactory.CurrentBall) return;

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
                _lineRenderer.positionCount = _trajectoryPoints;

                Color color = _ballFactory.BallColor;
                _lineRenderer.startColor = color;
                _lineRenderer.endColor = color;
            }

            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                _isDragging = false;
                _lineRenderer.positionCount = 0;

                ReleaseBall();
            }
        }

        private void UpdateTrajectory()
        {
            Vector2 dragDelta = (Input.mousePosition - _initialMousePosition) * _dragSensitivity;

            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;
            Vector3 cameraUp = _mainCamera.transform.up;

            Vector3 throwDir = (
                cameraForward * 30f +
                cameraRight * (dragDelta.x * 0.2f) +
                cameraUp * (dragDelta.y * _verticalMultiplier * 0.2f)
            ).normalized;

            float dragMagnitude = Mathf.Clamp(dragDelta.magnitude, 0, 200f);
            float forceMagnitude = Mathf.Max(_minimalForce, Mathf.Min(dragMagnitude, _maxForce));
            _throwDirection = throwDir * forceMagnitude / 1.5f;

            UpdateTrajectoryLine(_ballFactory.CurrentBallSpawnPoint, _throwDirection);
        }

        private void UpdateTrajectoryLine(Vector3 startPosition, Vector3 initialVelocity)
        {
            if (!_lineRenderer) return;

            var points = new Vector3[_trajectoryPoints];
            points[0] = startPosition;
            int actualPoints = _trajectoryPoints;

            for (var i = 1; i < _trajectoryPoints; i++)
            {
                float time = i * _timeStep;

                points[i] = CalculatePositionAtTime(startPosition, initialVelocity, time, Physics.gravity);

                if (Physics.Raycast(points[i - 1], (points[i] - points[i - 1]).normalized, out RaycastHit hit,
                        Vector3.Distance(points[i - 1], points[i]), _collisionMaskLine))
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

            _ballFactory.ReleaseBall(_throwDirection);
        }

        private bool IsMouseInDragArea()
        {
            Vector3 worldMousePosition = Input.mousePosition;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragArea, worldMousePosition, _uiCamera,
                out Vector3 worldPoint);

            Vector2 localMousePosition = _dragArea.InverseTransformPoint(worldPoint);

            return _dragArea.rect.Contains(localMousePosition);
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

        public void StartGame()
        {
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