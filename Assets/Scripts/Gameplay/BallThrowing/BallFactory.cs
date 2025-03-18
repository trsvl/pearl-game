using System.Collections;
using System.Linq;
using Bootstrap;
using Gameplay.Animations;
using Gameplay.SphereData;
using Gameplay.UI.Header;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using Random = UnityEngine.Random;

namespace Gameplay.BallThrowing
{
    public class BallFactory : MonoBehaviour
    {
        public Ball CurrentBall => _currentBall;
        public Ball NextBall => _nextBall;
        public Vector3 CurrentBallSpawnPoint => _currentBallSpawnPoint;
        public Color BallColor => _currentBall.GetColor();

        private Ball _ballPrefab;
        private ShotsData _shotsData;
        private EventBus _eventBus;
        private SpheresDictionary _spheresDictionary;
        private CameraController _cameraController;
        private IObjectResolver _container;

        private Color[] _levelColors => _spheresDictionary.GetLevelColors();
        private Color _previousColor;

        private Ball _currentBall;
        private Ball _nextBall;
        private float _ballSize;
        private Vector3 _currentBallSpawnPoint;
        private Vector3 _nextBallSpawnPoint;
        private int _currentBallCount;
        private bool _isRespawning;


        [Inject]
        public void Init(Ball ballPrefab, ShotsData shotsData, EventBus eventBus, SpheresDictionary spheresDictionary,
            CameraController cameraController, IObjectResolver container)
        {
            _ballPrefab = ballPrefab;
            _shotsData = shotsData;
            _eventBus = eventBus;
            _spheresDictionary = spheresDictionary;
            _cameraController = cameraController;
            _container = container;

            Init();
        }

        private void Init()
        {
            var initialBall = Instantiate(_ballPrefab);
            initialBall.gameObject.SetActive(false);

            Renderer ballRenderer = initialBall.GetComponent<Renderer>();
            Bounds bounds = ballRenderer.bounds;
            const float ballScale = 0.3f;
            Vector3 objectSizes = (bounds.max - bounds.min) / ballScale;

            _ballSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);

            Destroy(initialBall.gameObject);

            UpdateBallsPosition(0f);
        }

        public (GameObject currentBall, GameObject nextBall) InitBallsData()
        {
            SpawnCurrentBall();
            SpawnNextBall();

            return (_currentBall.gameObject, _nextBall.gameObject);
        }

        public void UpdateBallsPosition(float newFOV)
        {
            (Vector3 newBallPosition, Vector3 newNextBallPosition) =
                _cameraController.UpdateBallsPositionAndFOV(_ballSize, newFOV);

            _currentBallSpawnPoint = newBallPosition;
            _nextBallSpawnPoint = newNextBallPosition;

            if (_currentBall) _currentBall.transform.position = _currentBallSpawnPoint;
            if (_nextBall) _nextBall.transform.position = _nextBallSpawnPoint;
        }

        public void ReleaseBall(Vector3 direction)
        {
            _currentBall?.ApplyForce(direction);

            _shotsData.CurrentNumber -= 1;
            _currentBallCount -= 1;

            Destroy(_currentBall?.gameObject, 3f);

            _currentBall?.Release(_shotsData.CurrentNumber);

            _currentBall = null;

            _eventBus.RaiseEvent<IReleaseBall>(handler => handler.OnReleaseBall());
        }

        public void RespawnBall()
        {
            if (_nextBall) Destroy(_nextBall.gameObject);

            _isRespawning = true;
            SpawnNextBall();
        }

        public void SetCurrentBall()
        {
            _currentBall = _nextBall;
            _nextBall = null;
            StartCoroutine(SpawnNextBallDelay());
        }

        private void SpawnCurrentBall()
        {
            _currentBall = SpawnBall(_currentBallSpawnPoint);
        }

        private void SpawnNextBall()
        {
            _nextBall = SpawnBall(_nextBallSpawnPoint);
        }

        private Ball SpawnBall(Vector3 spawnPoint)
        {
            if (IsPreventedToSpawnBall()) return null;
            if (_levelColors.Length == 0) return null;

            if (_isRespawning) _isRespawning = false;
            else _currentBallCount += 1;

            Ball ball = Instantiate(_ballPrefab, spawnPoint, SphereRotation.GetQuaternion);

            var ballRenderer = ball.GetComponent<Renderer>();
            var ballCollider = ball.GetComponent<Collider>();
            var ballRigidbody = ball.GetComponent<Rigidbody>();

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor(AllColors.BaseColor, GenerateBallColor());
            ballRenderer.SetPropertyBlock(block);

            _container.Inject(ball);
            ball.Init(ballRenderer, ballCollider, ballRigidbody, block);

            return ball;
        }

        private Color GenerateBallColor()
        {
            if (_levelColors.Length == 1) return _levelColors[0];

            Color[] filteredColors = _levelColors
                .Where(color => color != _previousColor && color != _currentBall?.GetColor()).ToArray();
            int randomIndex = Random.Range(0, filteredColors.Length);

            Color newColor = filteredColors[randomIndex];
            _previousColor = newColor;

            return newColor;
        }

        private IEnumerator SpawnNextBallDelay()
        {
            yield return new WaitForSeconds(0.5f);
            if (!_nextBall) SpawnNextBall();
            if (_currentBall) _eventBus.RaiseEvent<IAfterReleaseBall>(handler => handler.OnAfterReleaseBall());
        }

        public bool IsPreventedToSpawnBall()
        {
            return _shotsData.CurrentNumber == _currentBallCount;
        }
    }
}