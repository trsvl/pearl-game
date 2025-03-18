using System.Text;
using Bootstrap.Currency;
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
        [SerializeField] private GameObject _currencyPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Loader>(Lifetime.Singleton)
                .WithParameter(_loadingScreenPrefab);

            builder.Register<PlayerData>(Lifetime.Singleton); //!!!

            builder.Register<CurrencyView>(Lifetime.Singleton)
                .WithParameter(_currencyPrefab)
                .WithParameter(new CurrencyConverter())
                .WithParameter(new StringBuilder());

            builder.Register<CurrencyModel>(Lifetime.Singleton);

            builder.Register<CurrencyController>(Lifetime.Singleton);

            builder.Register<CameraController>(Lifetime.Singleton);

            builder.Register<EventBus>(Lifetime.Singleton);

            AudioController audioController = new GameObject("Audio Manager").AddComponent<AudioController>();
            audioController.transform.SetParent(transform);
            builder.RegisterComponent(audioController).WithParameter(audioList.audios);

            builder.RegisterEntryPoint<BootstrapEntryPoint>();
        }
    }
}