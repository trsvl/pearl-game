using Dev.LevelBuilder;
using Utils.Scene.DI;
using VContainer;
using VContainer.Unity;

namespace Dev.DI
{
    public class LevelBuilderLifetimeScope : DefaultLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInHierarchy<UILevelBuilder>();
            
            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<UILevelBuilder>();
            });
        }
    }
}