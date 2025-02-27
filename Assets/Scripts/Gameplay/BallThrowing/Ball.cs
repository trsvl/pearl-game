using UnityEngine;
using Utils.SphereData;

namespace Gameplay.BallThrowing
{
    public class Ball : MonoBehaviour
    {
        public Renderer _renderer { get; private set; }

        private SphereDestroyer _sphereDestroyer;
        private Collider _collider;


        public void Init(SphereDestroyer sphereDestroyer)
        {
            _sphereDestroyer = sphereDestroyer;
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            transform.rotation = SphereRotation.GetQuaternion;
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
                    _sphereDestroyer.DestroySpheresSegment(collision, ballColor);
                    Destroy(gameObject);
                }
                else
                {
                    _collider.enabled = false;
                }
            }
        }
    }
}