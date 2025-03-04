using Utils.EventBusSystem;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Utils
{
    public class UtilsInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<CameraManager>(Lifetime.Scoped);
            builder.Register<GameplayStateObserver>(Lifetime.Scoped);
        }
    }
}