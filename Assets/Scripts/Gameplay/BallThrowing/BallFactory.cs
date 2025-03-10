using System.Collections;
using System.Linq;
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
        private CameraManager _cameraManager;
        private GameResultChecker _gameResultChecker;
        private IObjectResolver _container;

        private Color[] _levelColors => _spheresDictionary.GetLevelColors();
        private Vector3 _ballLocalScale;
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
            CameraManager cameraManager, GameResultChecker gameResultChecker, IObjectResolver container)
        {
            _ballPrefab = ballPrefab;
            _shotsData = shotsData;
            _eventBus = eventBus;
            _spheresDictionary = spheresDictionary;
            _cameraManager = cameraManager;
            _gameResultChecker = gameResultChecker;
            _container = container;
        }

        public (GameObject currentBall, GameObject nextBall) InitBallData()
        {
            var initialBall = Instantiate(_ballPrefab);
            initialBall.gameObject.SetActive(false);

            _ballLocalScale = _spheresDictionary.LowestSphereScale;

            initialBall.transform.localScale = Vector3.one;
            Renderer ballRenderer = initialBall.GetComponent<Renderer>();
            Bounds bounds = ballRenderer.bounds;
            const float ballScale = 0.3f;
            Vector3 objectSizes = (bounds.max - bounds.min) / ballScale;

            _ballSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);

            Destroy(initialBall.gameObject);

            UpdateBallPosition(0f);

            SpawnCurrentBall();
            SpawnNextBall();

            return (_currentBall.gameObject, _nextBall.gameObject);
        }

        public void UpdateBallPosition(float newFOV)
        {
            (Vector3 newBallPosition, Vector3 newNextBallPosition) =
                _cameraManager.UpdateBallsPositionAndFOV(_ballSize, newFOV);

            _currentBallSpawnPoint = newBallPosition;
            _nextBallSpawnPoint = newNextBallPosition;

            if (!_currentBall) return;

            _currentBall.transform.position = _currentBallSpawnPoint;
        }

        public void ReleaseBall(Vector3 direction)
        {
            _currentBall?.ApplyForce(direction);

            _shotsData.CurrentNumber -= 1;
            _currentBallCount -= 1;

            StartCoroutine(DestroyBallDelay(_currentBall?.gameObject));

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

            ball.transform.localScale = _ballLocalScale;

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

            Color[] filteredColors = _levelColors.Where(color => color != _previousColor).ToArray();
            int randomIndex = Random.Range(0, filteredColors.Length);

            Color newColor = filteredColors[randomIndex];
            _previousColor = newColor;

            return newColor;
        }

        private IEnumerator SpawnNextBallDelay()
        {
            yield return new WaitForSeconds(1.5f);
            if (!_nextBall) SpawnNextBall();
        }

        private IEnumerator DestroyBallDelay(GameObject ballObject)
        {
            yield return new WaitForSeconds(3f);

            Destroy(ballObject);
            _gameResultChecker.CheckGameResult();
        }

        public bool IsPreventedToSpawnBall()
        {
            return _shotsData.CurrentNumber == _currentBallCount;
        }
    }
}