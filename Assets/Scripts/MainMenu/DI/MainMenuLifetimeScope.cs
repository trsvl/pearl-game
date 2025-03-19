using MainMenu.UI.Footer;
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

            builder.RegisterComponentInHierarchy<MainMenuFooter>();

            builder.Register<MainMenuEventsHandler>(Lifetime.Scoped);

            builder.Register<MainMenuAudioEventsHandler>(Lifetime.Scoped).WithParameter(audioList.audios);

            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<MainMenuEventsHandler>();
                container.Resolve<MainMenuAudioEventsHandler>();
            });

            builder.RegisterEntryPoint<MainMenuEntryPoint>();
        }
    }
}