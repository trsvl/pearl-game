using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.Animations
{
    public class ParticlesFactory : IDestroySphere
    {
        private readonly ParticleSystem _onDestroySphereParticlePrefab;
        private IObjectPool<ParticleSystem> _onDestroySphereParticlesPool;
        private int count;


        public ParticlesFactory(ParticleSystem onDestroySphereParticlePrefab)
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

        public async void OnDestroySphere(GameObject sphere)
        {
            ParticleSystem particle = _onDestroySphereParticlesPool.Get();
            particle.transform.position = sphere.transform.position;
            particle.Play();
            await WaitForParticleToStop(particle);
            _onDestroySphereParticlesPool.Release(particle);
        }

        private async UniTask WaitForParticleToStop(ParticleSystem particle)
        {
            while (particle.IsAlive())
            {
                await UniTask.Yield();
            }
        }

        private ParticleSystem OnCreate()
        {
            count += 1;
            Debug.Log(count);
            var particle = Object.Instantiate(_onDestroySphereParticlePrefab);
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
            Object.Destroy(particle.gameObject);
        }
    }
}