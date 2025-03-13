using Gameplay.Utils;
using Utils.Scene.DI;
using VContainer;
using VContainer.Unity;

namespace Gameplay.DI
{
    public class GameplayLifetimeScope : DefaultLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new UtilsInstaller().Install(builder);

            builder.Register<GameplayEventsHandler>(Lifetime.Scoped);
            
            builder.Register<GameplayAudioEventsHandler>(Lifetime.Scoped).WithParameter(audioList.audios);

            base.Configure(builder);

            builder.UseEntryPoints(points =>
            {
                points.Add<GameplayEntryPoint>();
            });
            
            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<GameplayEventsHandler>();
                container.Resolve<GameplayAudioEventsHandler>();
            });
        }
    }
}