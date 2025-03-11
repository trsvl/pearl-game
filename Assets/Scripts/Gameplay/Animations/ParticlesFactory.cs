using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Gameplay.Animations
{
    public class ParticlesFactory : MonoBehaviour, IDestroySphere
    {
        private ParticleSystem _onDestroySphereParticlePrefab;

        private IObjectPool<ParticleSystem> _onDestroySphereParticlesPool;
        private int _count;


        [Inject]
        public void Init(ParticleSystem onDestroySphereParticlePrefab)
        {
            _onDestroySphereParticlePrefab = onDestroySphereParticlePrefab;
            InitializePool();
        }

        private void InitializePool()
        {
            const int capacity = 20;

            _onDestroySphereParticlesPool = new ObjectPool<ParticleSystem>(
                OnCreate,
                OnSpawn,
                OnRelease,
                OnDestroyObject,
                false,
                capacity,
                capacity
            );
        }

        public void OnDestroySphere(GameObject sphere)
        {
            if (!sphere) return;
            ParticleSystem particle = _onDestroySphereParticlesPool.Get();
            particle.transform.position = sphere.transform.position;
            particle.Play();
            StartCoroutine(WaitForParticleToStop(particle));
        }

        private IEnumerator WaitForParticleToStop(ParticleSystem particle)
        {
            yield return new WaitUntil(() => !particle.IsAlive());
            _onDestroySphereParticlesPool.Release(particle);
        }

        private ParticleSystem OnCreate()
        {
            _count += 1;
           // Debug.Log(_count);
            var particle = Instantiate(_onDestroySphereParticlePrefab);
            particle.gameObject.SetActive(false);
            return particle;
        }

        private void OnSpawn(ParticleSystem particle)
        {
            particle.gameObject.SetActive(true);
        }

        private void OnRelease(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);
        }

        private void OnDestroyObject(ParticleSystem particle)
        {
            Destroy(particle.gameObject);
        }
    }
}