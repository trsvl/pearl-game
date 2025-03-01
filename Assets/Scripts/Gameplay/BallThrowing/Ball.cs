using Gameplay.Actions;
using Gameplay.SphereData;
using UnityEngine;

namespace Gameplay.BallThrowing
{
    public class Ball : MonoBehaviour
    {
        private OnDestroySphereSegment _onDestroySphereSegment;
        private Renderer _renderer;
        private Collider _collider;
        private Rigidbody _rigidbody;


        public void Init(OnDestroySphereSegment onDestroySphereSegment, Renderer ballRenderer, Collider ballCollider,
            Rigidbody ballRigidbody)
        {
            _onDestroySphereSegment = onDestroySphereSegment;
            _renderer = ballRenderer;
            _collider = ballCollider;
            _rigidbody = ballRigidbody;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                var materialPropertyBlock = new MaterialPropertyBlock();
                Renderer sphereRenderer = collision.gameObject.GetComponent<Renderer>();
                sphereRenderer.GetPropertyBlock(materialPropertyBlock);

                Color ballColor = _renderer.sharedMaterial.GetColor(AllColors.BaseColor);
                Color touchedSphereColor = materialPropertyBlock.GetColor(AllColors.BaseColor);

                if (ballColor == touchedSphereColor)
                {
                    _onDestroySphereSegment.NotifyAll(ballColor, collision.gameObject);
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