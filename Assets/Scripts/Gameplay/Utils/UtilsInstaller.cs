using VContainer;
using VContainer.Unity;

namespace Gameplay.Utils
{
    public class UtilsInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<GameplayStateObserver>(Lifetime.Scoped);

            builder.Register<GameResultChecker>(Lifetime.Scoped);
        }
    }
}