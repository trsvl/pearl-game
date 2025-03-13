using System.Collections.Generic;
using UnityEngine;
using Utils.Scene.AudioSystem;
using VContainer;
using VContainer.Unity;

namespace Utils.Scene.DI
{
    public abstract class BaseLifetimeScope : LifetimeScope
    {
        [SerializeField] protected List<GameObject> _installers;
        [SerializeField] protected AudioList audioList;


        protected override void Configure(IContainerBuilder builder)
        {
            foreach (GameObject gameObj in _installers)
            {
                var installer = gameObj.GetComponent<IInstaller>();
                installer?.Install(builder);
            }
        }
    }
}