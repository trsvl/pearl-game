using System.Threading;
using Gameplay.Animations;
using MainMenu.UI.Header;
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
        [SerializeField] private GameObject _goldIconPrefab;
        [SerializeField] private GameObject _diamondIconPrefab;
        [SerializeField] private MainMenuHeader _mainMenuHeaderPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Loader>(Lifetime.Singleton)
                .WithParameter(_loadingScreenPrefab);

            builder.Register<PlayerData>(Lifetime.Singleton);

            builder.Register<CameraManager>(Lifetime.Singleton).WithParameter(Camera.main); //!!!

            builder.Register<Currencies>(Lifetime.Singleton)
                .WithParameter("goldIconPrefab", _goldIconPrefab)
                .WithParameter("diamondIconPrefab", _diamondIconPrefab)
                .WithParameter(_mainMenuHeaderPrefab);

            builder.RegisterComponent(_mainMenuHeaderPrefab);

            builder.Register<CurrencyConverter>(Lifetime.Singleton);

            CancellationTokenSource tokenSource = new CancellationTokenSource(); //!!!
            builder.Register<CurrencyAnimation>(Lifetime.Singleton).WithParameter(tokenSource.Token);

            builder.Register<EventBus>(Lifetime.Singleton);

            AudioManager audioManager = new GameObject("Audio Manager").AddComponent<AudioManager>();
            audioManager.transform.SetParent(transform);
            builder.RegisterComponent(audioManager)
                .WithParameter(audioList.audios);

            builder.RegisterEntryPoint<BootstrapEntryPoint>();
        }
    }
}