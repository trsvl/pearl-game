using System.Collections;
using System.Linq;
using Gameplay.Actions;
using Gameplay.Header;
using Gameplay.SphereData;
using UnityEngine;

namespace Gameplay.BallThrowing
{
    public class BallFactory : MonoBehaviour, IStartGame
    {
        public Ball CurrentBall => _currentBall;
        public float BallSize => _ballSize;
        public Vector3 BallSpawnPoint => _ballSpawnPoint;
        public Color BallColor => _currentBall.GetColor();

        private Ball _ballPrefab;
        private ShotsData _shotsData;
        private OnDestroySphereSegment _onDestroySphereSegment;
        private Color[] _levelColors;
        private Vector3 _ballLocalScale;
        private Color _previousColor;

        private Ball _currentBall;
        private float _ballSize;
        private Vector3 _ballSpawnPoint;


        public void Init(Ball ballPrefab, ShotsData shotsData,
            OnDestroySphereSegment onDestroySphereSegment, Vector3 ballLocalScale)
        {
            _ballPrefab = ballPrefab;
            _shotsData = shotsData;
            _onDestroySphereSegment = onDestroySphereSegment;

            InitBallData(ballLocalScale);
        }

        private void InitBallData(Vector3 ballLocalScale)
        {
            _ballLocalScale = ballLocalScale;

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
            _currentBall?.ApplyForce(direction);
            _shotsData.CurrentNumber -= 1;

            StartCoroutine(DestroyBallAfterDelay(_currentBall?.gameObject));
            StartCoroutine(SpawnBallAfterDelay(1f));

            _currentBall = null;
        }

        public void RespawnBall()
        {
            if (_currentBall != null) Destroy(_currentBall.gameObject);

            SpawnBall();
        }

        private void SpawnBall()
        {
            if (_shotsData.CurrentNumber <= 0) return;

            _currentBall = Instantiate(_ballPrefab, _ballSpawnPoint, SphereRotation.GetQuaternion);

            var ballRenderer = _currentBall.GetComponent<Renderer>();
            var ballCollider = _currentBall.GetComponent<Collider>();
            var ballRigidbody = _currentBall.GetComponent<Rigidbody>();
            _currentBall.Init(_onDestroySphereSegment, ballRenderer, ballCollider, ballRigidbody);

            _currentBall.transform.localScale = _ballLocalScale;
            ballRenderer.material.SetColor(AllColors.BaseColor, GenerateBallColor());
        }

        private Color GenerateBallColor()
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

        private IEnumerator DestroyBallAfterDelay(GameObject ball)
        {
            yield return new WaitForSeconds(4f);

            Destroy(ball);
            _currentBall = null;
        }

        public void StartGame()
        {
            SpawnBall(); //!!!
        }
    }
}