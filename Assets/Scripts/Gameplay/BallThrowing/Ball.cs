using Gameplay.SphereData;
using UnityEngine;
using Utils.EventBusSystem;

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


        public void Init(EventBus eventBus, Renderer ballRenderer, Collider ballCollider, Rigidbody ballRigidbody,
            MaterialPropertyBlock materialPropertyBlock)
        {
            _eventBus = eventBus;
            _renderer = ballRenderer;
            _collider = ballCollider;
            _rigidbody = ballRigidbody;
            _materialPropertyBlock = materialPropertyBlock;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isTouchedSphere && collision.gameObject.CompareTag("Ball"))
            {
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
                    Destroy(gameObject);
                }
                else
                {
                    _collider.enabled = false;
                }
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