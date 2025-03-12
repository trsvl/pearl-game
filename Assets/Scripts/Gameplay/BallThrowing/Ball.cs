using Gameplay.SphereData;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;

namespace Gameplay.BallThrowing
{
    public class Ball : MonoBehaviour
    {
        private EventBus _eventBus;
        private GameResultChecker _gameResultChecker;

        private Renderer _renderer;
        private Collider _collider;
        private Rigidbody _rigidbody;
        private bool _isTouchedSphere;
        private bool _isTouchedRightSphere;
        private MaterialPropertyBlock _materialPropertyBlock;
        private int _currentShotsNumber = -1;


        [Inject]
        public void Construct(EventBus eventBus, GameResultChecker gameResultChecker)
        {
            _eventBus = eventBus;
            _gameResultChecker = gameResultChecker;
        }

        public void Init(Renderer ballRenderer, Collider ballCollider, Rigidbody ballRigidbody,
            MaterialPropertyBlock materialPropertyBlock)
        {
            _renderer = ballRenderer;
            _collider = ballCollider;
            _rigidbody = ballRigidbody;
            _materialPropertyBlock = materialPropertyBlock;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isTouchedSphere || !collision.gameObject.CompareTag("Ball")) return;

            _isTouchedSphere = true;

            Renderer sphereRenderer = collision.gameObject.GetComponent<Renderer>();
            sphereRenderer.GetPropertyBlock(_materialPropertyBlock);
            Color touchedSphereColor = _materialPropertyBlock.GetColor(AllColors.BaseColor);

            _renderer.GetPropertyBlock(_materialPropertyBlock);
            Color ballColor = _materialPropertyBlock.GetColor(AllColors.BaseColor);

            if (ballColor == touchedSphereColor)
            {
                _isTouchedRightSphere = true;

                _eventBus.RaiseEvent<IDestroySphereSegmentOnHit>(handler =>
                    handler.OnDestroySphereSegmentOnHit(ballColor, collision.gameObject, _currentShotsNumber));

                Destroy(gameObject);
            }
            else
            {
                _collider.enabled = false;
            }
        }

        public Color GetColor()
        {
            _renderer.GetPropertyBlock(_materialPropertyBlock);
            return _materialPropertyBlock.GetColor(AllColors.BaseColor);
        }

        public void ApplyForce(Vector3 force)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public void Release(int currentShotsNumber)
        {
            _currentShotsNumber = currentShotsNumber;
        }

        private void OnDestroy()
        {
            if (!_isTouchedRightSphere) _gameResultChecker?.CheckGameResult(_currentShotsNumber);
        }
    }
}