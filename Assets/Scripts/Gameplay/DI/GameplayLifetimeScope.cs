using System.Collections.Generic;
using System.Threading;
using Gameplay.Animations;
using Gameplay.Utils;
using UnityEngine;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private List<GameObject> _installers;

        private readonly CancellationTokenSource _cts = new();


        protected override void Configure(IContainerBuilder builder)
        {
            foreach (GameObject gameObj in _installers)
            {
                var installer = gameObj.GetComponent<IInstaller>();
                installer?.Install(builder);
            }

            new UtilsInstaller().Install(builder);
            
            builder.RegisterComponent(_cts.Token);

            builder.UseEntryPoints(points =>
            {
                points.Add<GameplaySubscriber>();
                points.Add<GameplayEntryPoint>();
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _cts.Cancel();
            _cts.Dispose();
        }
    }
}