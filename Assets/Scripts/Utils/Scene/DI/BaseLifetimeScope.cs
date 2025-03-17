using UnityEngine;
using Utils.Scene.AudioSystem;
using VContainer;
using VContainer.Unity;

namespace Utils.Scene.DI
{
    public abstract class BaseLifetimeScope : LifetimeScope
    {
        [SerializeField] protected AudioList audioList;


        protected override void Configure(IContainerBuilder builder)
        {
            var installers = GetComponentsInChildren<IInstaller>();
            
            foreach (IInstaller installer in installers)
            {
                installer.Install(builder);
            }
        }
    }
}