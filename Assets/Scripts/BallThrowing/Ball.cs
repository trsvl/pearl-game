using UnityEngine;

namespace BallThrowing
{
    public class Ball : MonoBehaviour
    {
        private SphereDestroyer _sphereDestroyer;


        public void Init(SphereDestroyer sphereDestroyer)
        {
            _sphereDestroyer = sphereDestroyer;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                _sphereDestroyer.TryInitiateWave(collision);
                Destroy(gameObject);
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