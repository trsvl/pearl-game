using System.Collections.Generic;
using UnityEngine;
using Utils.DI;
using VContainer;
using VContainer.Unity;

namespace MainMenu.DI
{
    public class MainMenuLifetimeScope : BaseLifetimeScope
    {
        [SerializeField] private List<GameObject> _installers;


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            foreach (GameObject gameObj in _installers)
            {
                var installer = gameObj.GetComponent<IInstaller>();
                installer?.Install(builder);
            }

            builder.RegisterComponentInHierarchy<MainMenuManager>();

            builder.RegisterEntryPoint<MainMenuEntryPoint>();
        }
    }
}