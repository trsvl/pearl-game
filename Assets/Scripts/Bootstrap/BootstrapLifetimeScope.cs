using UnityEngine;
using Utils.EventBusSystem;
using Utils.Scene.DI;
using VContainer;
using VContainer.Unity;

namespace Bootstrap
{
    public class BootstrapLifetimeScope : BaseLifetimeScope
    {
        [SerializeField] private GameObject _loadingScreenPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Loader>(Lifetime.Singleton)
                .WithParameter(_loadingScreenPrefab);

            builder.Register<PlayerData>(Lifetime.Singleton);

            builder.Register<EventBus>(Lifetime.Singleton);

            AudioManager audioManager = new GameObject("Audio Manager").AddComponent<AudioManager>();
            audioManager.transform.SetParent(transform);
            builder.RegisterComponent(audioManager)
                .WithParameter(audioList.audios);

            builder.RegisterEntryPoint<BootstrapEntryPoint>();
        }
    }
}