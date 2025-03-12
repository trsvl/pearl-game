using Bootstrap.AudioSystem;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Bootstrap
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameObject _loadingScreenPrefab;
        [SerializeField] private AudioManager _audioManagerPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Loader>(Lifetime.Singleton)
                .WithParameter(_loadingScreenPrefab);

            builder.Register<PlayerData>(Lifetime.Singleton);

            builder.Register<EventBus>(Lifetime.Singleton);

           // builder.RegisterComponentInNewPrefab(_audioManagerPrefab, Lifetime.Singleton).DontDestroyOnLoad();

            builder.RegisterEntryPoint<BootstrapEntryPoint>();
        }
    }
}