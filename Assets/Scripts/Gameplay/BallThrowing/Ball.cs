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
        private Renderer _renderer;
        private Collider _collider;
        private Rigidbody _rigidbody;
        private bool _isTouchedSphere;
        private MaterialPropertyBlock _materialPropertyBlock;


        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
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
                _eventBus.RaiseEvent<IDestroySphereSegment>(handler =>
                    handler.OnDestroySphereSegment(ballColor, collision.gameObject));
                gameObject.SetActive(false);
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
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}