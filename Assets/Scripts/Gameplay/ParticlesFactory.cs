using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Gameplay
{
    public class ParticlesFactory : MonoBehaviour, IDestroySphere
    {
        [SerializeField] private ParticleSystem _onDestroySphereParticlePrefab;

        private int count = 0;
        private IObjectPool<ParticleSystem> _onDestroySphereParticlesPool;


        [Inject]
        private void Init()
        {
            int capacity = 20;
            
            _onDestroySphereParticlesPool = new ObjectPool<ParticleSystem>(
                OnCreate,
                OnSpawn,
                OnRelease,
                OnDestroyObject,
                false,
                capacity,
                capacity
            );

            for (int i = 0; i < capacity; i++)
            {
                ParticleSystem particle = _onDestroySphereParticlesPool.Get();
                _onDestroySphereParticlesPool.Release(particle);
            }
        }

        public async void OnDestroySphere(GameObject sphere)
        {
            count += 1;
            Debug.Log(count);
            ParticleSystem particle = _onDestroySphereParticlesPool.Get();
            particle.transform.position = sphere.transform.position;
            particle.Play();
            await WaitForParticleToStop(particle);
            _onDestroySphereParticlesPool.Release(particle);
        }

        private async Task WaitForParticleToStop(ParticleSystem particle)
        {
            while (particle.IsAlive())
            {
                await Task.Yield();
            }
        }

        private ParticleSystem OnCreate()
        {
            var particle = Instantiate(_onDestroySphereParticlePrefab, transform);
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