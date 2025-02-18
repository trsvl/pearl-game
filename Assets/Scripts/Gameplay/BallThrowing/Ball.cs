using UnityEngine;
using Utils.Colors;

namespace Gameplay.BallThrowing
{
    public class Ball : MonoBehaviour
    {
        public Renderer _renderer { get; private set; }

        private SphereDestroyer _sphereDestroyer;
        private LayerMask _layerMask;
        private Collider _collider;


        public void Init(SphereDestroyer sphereDestroyer, LayerMask layerMask)
        {
            _sphereDestroyer = sphereDestroyer;
            _layerMask = layerMask;
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                var touchedBallMaterial = collision.gameObject.GetComponent<Renderer>().material;
                if (_renderer.material.GetColor(AllColors.BaseColor) == touchedBallMaterial.GetColor(AllColors.BaseColor))
                {
                    _sphereDestroyer.TryInitiateWave(collision);
                    Destroy(gameObject);
                }
                else
                {
                    _collider.excludeLayers += _layerMask;
                }
            }
        }

        public void InvokeDestroy()
        {
            Invoke(nameof(SelfDestruct), 5f);
        }

        private void SelfDestruct()
        {
            Destroy(gameObject);
        }
    }
}