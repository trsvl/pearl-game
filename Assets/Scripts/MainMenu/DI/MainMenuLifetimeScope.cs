using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainMenu.DI
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private List<GameObject> _installers;

        
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Configuring MainMenuLifetimeScope");
            foreach (GameObject gameObj in _installers)
            {
                var installer = gameObj.GetComponent<IInstaller>();
                installer?.Install(builder);
            }

            builder.RegisterComponentInHierarchy<MainMenuManager>();
        }
    }
}