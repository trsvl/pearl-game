using MainMenu.UI.Header;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainMenu.UI
{
    public class UIInstaller : MonoBehaviour, IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MainMenuHeaderManager>();

           // builder.RegisterBuildCallback(container => { container.Resolve<MainMenuHeader>(); });
        }
    }
}