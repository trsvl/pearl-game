using System.Collections.Generic;
using System.Threading;
using Bootstrap;
using Bootstrap.AudioSystem;
using Gameplay.Animations;
using Gameplay.Utils;
using UnityEngine;
using Utils.DI;
using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayLifetimeScope : BaseLifetimeScope
    {
        [SerializeField] private List<GameObject> _installers;
        [SerializeField] private AudioManager _audioManagerPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            foreach (GameObject gameObj in _installers)
            {
                var installer = gameObj.GetComponent<IInstaller>();
                installer?.Install(builder);
            }

            new UtilsInstaller().Install(builder);
            
            builder.RegisterComponentInNewPrefab(_audioManagerPrefab, Lifetime.Scoped); //!!!

            builder.UseEntryPoints(points =>
            {
                points.Add<GameplaySubscriber>();
                points.Add<GameplayEntryPoint>();
            });
        }
    }
}