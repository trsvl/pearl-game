using System.Collections;
using System.Linq;
using Gameplay.SphereData;
using Gameplay.UI.Header;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using Random = UnityEngine.Random;

namespace Gameplay.BallThrowing
{
    public class BallFactory : MonoBehaviour
    {
        public Ball CurrentBall => _currentBall;
        public float BallSize => _ballSize;
        public Vector3 BallSpawnPoint => _ballSpawnPoint;
        public Color BallColor => _currentBall.GetColor();

        private Ball _ballPrefab;
        private ShotsData _shotsData;
        private EventBus _eventBus;
        private Color[] _levelColors;
        private Vector3 _ballLocalScale;
        private Color _previousColor;
        private SphereGenerator _sphereGenerator;

        private Ball _currentBall;
        private float _ballSize;
        private Vector3 _ballSpawnPoint;


        [Inject]
        public void Init(Ball ballPrefab, ShotsData shotsData,
            EventBus eventBus)
        {
            _ballPrefab = ballPrefab;
            _shotsData = shotsData;
            _eventBus = eventBus;
        }

        public void ReInit(Vector3 ballLocalScale, Color[] levelColors)
        {
            _ballLocalScale = ballLocalScale;
            _levelColors = levelColors;

            InitBallData();
        }

        private void InitBallData()
        {
            var initialBall = Instantiate(_ballPrefab);
            initialBall.gameObject.SetActive(false);

            initialBall.transform.localScale = _ballLocalScale;
            Renderer ballRenderer = initialBall.GetComponent<Renderer>();
            Bounds bounds = ballRenderer.bounds;
            const float ballScale = 0.3f;
            Vector3 objectSizes = (bounds.max - bounds.min) / ballScale;

            _ballSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);

            Destroy(initialBall.gameObject);
        }

        public void UpdateData(Vector3 newBallPosition)
        {
            _ballSpawnPoint = newBallPosition;

            if (!_currentBall) return;

            _currentBall.transform.position = _ballSpawnPoint;
        }

        public void ReleaseBall(Vector3 direction)
        {
            print("Releasing ball");
            _currentBall?.ApplyForce(direction);
            _shotsData.CurrentNumber -= 1;

            StartCoroutine(DestroyBallAfterDelay(_currentBall?.gameObject));
            StartCoroutine(SpawnBallAfterDelay(1f));

            _currentBall = null;
        }

        public void RespawnBall()
        {
            if (_currentBall) Destroy(_currentBall.gameObject);

            SpawnBall();
        }

        public GameObject GetBall()
        {
            return _currentBall != null ? _currentBall.gameObject : SpawnBall();
        }

        private GameObject SpawnBall()
        {
            if (_shotsData.CurrentNumber <= 0) return null;

            _currentBall = Instantiate(_ballPrefab, _ballSpawnPoint, SphereRotation.GetQuaternion);

            var ballRenderer = _currentBall.GetComponent<Renderer>();
            var ballCollider = _currentBall.GetComponent<Collider>();
            var ballRigidbody = _currentBall.GetComponent<Rigidbody>();
            _currentBall.Init(_eventBus, ballRenderer, ballCollider, ballRigidbody);

            _currentBall.transform.localScale = _ballLocalScale;
            ballRenderer.material.SetColor(AllColors.BaseColor, GenerateBallColor());

            return _currentBall.gameObject;
        }

        private Color GenerateBallColor()
        {
            if (_levelColors.Length <= 1) return _levelColors[0];

            Color[] filteredColors = _levelColors.Where(color => color != _previousColor).ToArray();
            int randomIndex = Random.Range(0, filteredColors.Length);
            Color newColor = _levelColors[randomIndex];
            _previousColor = newColor;
            
            return newColor;
        }

        private IEnumerator SpawnBallAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            RespawnBall();
        }

        private IEnumerator DestroyBallAfterDelay(GameObject ball)
        {
            yield return new WaitForSeconds(4f);

            Destroy(ball);
        }
    }
}