using Utils.Scene.DI;
using VContainer;
using VContainer.Unity;

namespace MainMenu.DI
{
    public class MainMenuLifetimeScope : DefaultLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<MainMenuManager>();

            builder.RegisterEntryPoint<MainMenuEntryPoint>();
        }
    }
}