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


        public void Init(EventBus eventBus, Renderer ballRenderer, Collider ballCollider,
            Rigidbody ballRigidbody)
        {
            _eventBus = eventBus;
            _renderer = ballRenderer;
            _collider = ballCollider;
            _rigidbody = ballRigidbody;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isTouchedSphere && collision.gameObject.CompareTag("Ball"))
            {
                _isTouchedSphere = true;

                var materialPropertyBlock = new MaterialPropertyBlock();
                Renderer sphereRenderer = collision.gameObject.GetComponent<Renderer>();
                sphereRenderer.GetPropertyBlock(materialPropertyBlock);

                Color ballColor = _renderer.sharedMaterial.GetColor(AllColors.BaseColor);
                Color touchedSphereColor = materialPropertyBlock.GetColor(AllColors.BaseColor);

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
            return _renderer.material.GetColor(AllColors.BaseColor);
        }

        public void ApplyForce(Vector3 force)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}