using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Bootstrap
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameObject _loadingScreenPrefab;


        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Loader>(Lifetime.Singleton)
                .WithParameter(_loadingScreenPrefab);

            builder.Register<PlayerData>(Lifetime.Singleton);

            builder.RegisterEntryPoint<BootstrapEntryPoint>(Lifetime.Scoped);
        }
    }
}